using AirportRouteApi.Models;
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
        private static ConcurrentDictionary<int, CancellationTokenSource> concurrentDictionary = new ConcurrentDictionary<int, CancellationTokenSource>();

        public async Task<Result> TrySetTask(string from, string to, int maxTransferCount, string userAgent, string remoteAddress)
        {
            var tokenSource = new CancellationTokenSource();
            int hash = RouteHelper.GetHashCode(from, to, userAgent, remoteAddress);
            try
            {
                concurrentDictionary.TryAdd(hash, tokenSource);
                var taskResult = await ValidateInput(from, to, tokenSource.Token);
                if (!string.IsNullOrEmpty(taskResult))
                {
                    return new Result() { Error = taskResult };
                }
                var route = await apiClient.GetRoutesByAirports(from, to, maxTransferCount, tokenSource.Token);
                return new Result() { Routes = route };
            }
            finally
            {
                concurrentDictionary.TryRemove(hash, out tokenSource);
            }
        }

        public Result CancelTask(string from, string to, string userAgent, string remoteAddress)
        {
            CancellationTokenSource tokenSource;
            int hash = RouteHelper.GetHashCode(from, to, userAgent, remoteAddress);
            try
            {
                concurrentDictionary.TryGetValue(hash, out tokenSource);

                bool tokenExistsAndCanBeCancelled = tokenSource != null && tokenSource.Token != null && tokenSource.Token.CanBeCanceled;
                if (tokenExistsAndCanBeCancelled)
                {
                    tokenSource.Cancel();
                }

                string result = tokenExistsAndCanBeCancelled ? ErrorMessages.ProcessWasStoped : ErrorMessages.HandlingProcessNotFound;
                return new Result() { Message = result };
            }
            finally
            {
                concurrentDictionary.TryRemove(hash, out tokenSource);
            }
        }

        public int ProcessingsTasksCount()
        {
            return concurrentDictionary.Count;
        }


        private async Task<string> ValidateInput(string from, string to, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return ErrorMessages.EmptyCodes;
            }
            if (!await apiClient.IsValidAirport(from, ct))
            {
                return ErrorMessages.NotValidSourceAirportCode;
            }
            if (!await apiClient.IsValidAirport(to, ct))
            {
                return ErrorMessages.NotValidSourceDestinationCode;
            }
            return string.Empty;
        }
    }
}
