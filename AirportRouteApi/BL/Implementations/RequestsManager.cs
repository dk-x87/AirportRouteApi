using AirportRouteApi.Messages;
using AirportRouteApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL.Implementations
{
    public class RequestsManager : IRequestsManager
    {
        public RequestsManager(IApiClient apiClnt, RequestParams requestParams, ILogger<RequestsManager> log)
        {
            apiClient = apiClnt;
            logger = log;
            maxConcurrentRequestsSettings = requestParams.MaxConcurrentRequests;
            maxTransferCountSettings = requestParams.MaxTransferCount;
        }

        private readonly IApiClient apiClient;
        private readonly ILogger logger;
        private readonly int maxConcurrentRequestsSettings;
        private readonly int maxTransferCountSettings;
        private static ConcurrentDictionary<int, CancellationTokenSource> concurrentDictionary = new ConcurrentDictionary<int, CancellationTokenSource>();

        public async Task<Responce<List<Route>>> TrySetTask(string from, string to, int maxTransferCount, string userAgent, string remoteAddress)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return Responce<List<Route>>.Fault(Error.GetConflictErrorResult(ErrorMessages.EmptyCodes));
            }

            var fromAirport = from.ToUpper();
            var toAirport = to.ToUpper();
            if (concurrentDictionary.Count >= maxConcurrentRequestsSettings)
            {
                return Responce<List<Route>>.Fault(Error.GetTooManyRequestsResult(ErrorMessages.ConcurrentRequestLimitExceeded));
            }
            var tokenSource = new CancellationTokenSource();
            int hash = RouteHelper.GetHashCode(fromAirport, toAirport, userAgent, remoteAddress);
            try
            {
                concurrentDictionary.TryAdd(hash, tokenSource);
                if (!await apiClient.IsValidAirport(fromAirport, tokenSource.Token))
                {
                    return Responce<List<Route>>.Fault(Error.GetConflictErrorResult(ErrorMessages.NotValidSourceAirportCode));
                }
                if (!await apiClient.IsValidAirport(toAirport, tokenSource.Token))
                {
                    return Responce<List<Route>>.Fault(Error.GetConflictErrorResult(ErrorMessages.NotValidSourceDestinationCode));
                }
                var routes = await apiClient.GetRoutesByAirports(fromAirport, toAirport, maxTransferCount == 0 ? maxTransferCountSettings : maxTransferCount, tokenSource.Token);
                return Responce<List<Route>>.Success(routes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                return Responce<List<Route>>.Fault(Error.GetInnerErrorResult(ex));
            }
            finally
            {
                concurrentDictionary.TryRemove(hash, out tokenSource);
            }
        }

        public Responce<string> CancelTask(string from, string to, string userAgent, string remoteAddress)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return Responce<string>.Fault(Error.GetConflictErrorResult(ErrorMessages.EmptyCodes));
            }

            CancellationTokenSource tokenSource = null;
            int hash = RouteHelper.GetHashCode(from.ToUpper(), to.ToUpper(), userAgent, remoteAddress);
            try
            {
                concurrentDictionary.TryGetValue(hash, out tokenSource);

                bool tokenExistsAndCanBeCancelled = tokenSource != null && tokenSource.Token != null && tokenSource.Token.CanBeCanceled;
                if (tokenExistsAndCanBeCancelled)
                {
                    tokenSource.Cancel();
                }

                return tokenExistsAndCanBeCancelled
                    ? Responce<string>.Success(SuccessMessages.ProcessWasStoped)
                    : Responce<string>.Fault(Error.GetConflictErrorResult(ErrorMessages.HandlingProcessNotFound));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                return Responce<string>.Fault(Error.GetInnerErrorResult(ex));
            }
            finally
            {
                concurrentDictionary.TryRemove(hash, out tokenSource);
            }
        }

    }
}
