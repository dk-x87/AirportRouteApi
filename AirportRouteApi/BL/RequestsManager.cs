using AirportRouteApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public class RequestsManager : IRequestsManager
    {
        public RequestsManager(IApiClient apiClnt)
        {
            apiClient = apiClnt;
        }

        private readonly IApiClient apiClient;
        private readonly int maxConcurrentRequests;
        private static ConcurrentDictionary<ConcurrentRoute, CancellationToken> concurrentDictionary = new ConcurrentDictionary<ConcurrentRoute, CancellationToken>();

        public async Task<string> TrySetTask(string from, string to, string userAgent, string remoteAddress)
        {
            try
            {
                var tokenSource = new CancellationTokenSource();
                CancellationToken token = tokenSource.Token;
                ConcurrentRoute concurrentRoute = new ConcurrentRoute() { SrcAirport = from, DestAirport = to, UserAgent = userAgent, RemoteAddress = remoteAddress, Token = token};
                return await TryGetRoutes(concurrentRoute);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CancelTask(string from, string to, string userAgent, string remoteAddress)
        {
            try
            {
                ConcurrentRoute concurrentRoute = new ConcurrentRoute() { SrcAirport = from, DestAirport = to, UserAgent = userAgent, RemoteAddress = remoteAddress };
                return TryCancelTask(concurrentRoute); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int ProcessingsTasksCount()
        {
            return concurrentDictionary.Count;
        }


        private async Task<string> TryGetRoutes(ConcurrentRoute concurrentRoute)
        {
            concurrentDictionary.TryAdd(concurrentRoute, concurrentRoute.Token);

            string error = ValidateInput(concurrentRoute.SrcAirport, concurrentRoute.DestAirport, concurrentRoute.Token);
            if (!string.IsNullOrEmpty(error))
            {
                return JsonConvert.SerializeObject(new Result() { Error = error });
            }
            var routes = await apiClient.GetRoutesByAirports(concurrentRoute.SrcAirport, concurrentRoute.DestAirport, concurrentRoute.Token);

            CancellationToken token;
            concurrentDictionary.TryRemove(concurrentRoute, out token);

            return JsonConvert.SerializeObject(new Result() { Routes = routes });
        }

        private string TryCancelTask(ConcurrentRoute concurrentRoute)
        {
            CancellationToken token;
            concurrentDictionary.TryGetValue(concurrentRoute, out token);

            bool tokenExistsAndCanBeCancelled = token != null && token.CanBeCanceled;
            if (tokenExistsAndCanBeCancelled)
            {
                token.ThrowIfCancellationRequested();
            }

            concurrentDictionary.TryRemove(concurrentRoute, out token);

            string result = tokenExistsAndCanBeCancelled ? ErrorMessages.ProcessWasStoped : ErrorMessages.HandlingProcessNotFound;
            return JsonConvert.SerializeObject(new Result() { Message = result });
        }

        private string ValidateInput(string from, string to, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return ErrorMessages.EmptyCodes;
            }
            if (!apiClient.IsValidAirport(from, ct).Result)
            {
                return ErrorMessages.NotValidSourceAirportCode;
            }
            if (!apiClient.IsValidAirport(to, ct).Result)
            {
                return ErrorMessages.NotValidSourceDestinationCode;
            }
            return string.Empty;
        }
    }
}
