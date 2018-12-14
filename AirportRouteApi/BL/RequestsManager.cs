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

        public async Task<Result> TrySetTask(string from, string to, string userAgent, string remoteAddress)
        {
            var tokenSource = new CancellationTokenSource();
            ConcurrentRoute concurrentRoute = new ConcurrentRoute() { SrcAirport = from, DestAirport = to, UserAgent = userAgent, RemoteAddress = remoteAddress, TokenSource = tokenSource};
            return await TryGetRoutes(concurrentRoute);
        }

        public Result CancelTask(string from, string to, string userAgent, string remoteAddress)
        {
            ConcurrentRoute concurrentRoute = new ConcurrentRoute() { SrcAirport = from, DestAirport = to, UserAgent = userAgent, RemoteAddress = remoteAddress };
            return TryCancelTask(concurrentRoute); 
        }

        public int ProcessingsTasksCount()
        {
            return concurrentDictionary.Count;
        }


        private async Task<Result> TryGetRoutes(ConcurrentRoute concurrentRoute)
        {
            CancellationTokenSource tokenSource;
            var task = ValidateInput(concurrentRoute.SrcAirport, concurrentRoute.DestAirport, concurrentRoute.TokenSource.Token);
            try
            {
                concurrentDictionary.TryAdd(concurrentRoute.GetHashCode(), concurrentRoute.TokenSource);
                await task;
                string error = task.Result;
                if (!string.IsNullOrEmpty(error))
                {
                    return new Result() { Error = error };
                }
                var route = await apiClient.GetRoutesByAirports(concurrentRoute.SrcAirport, concurrentRoute.DestAirport, concurrentRoute.TokenSource.Token);
                return new Result() { Route = route };
            }
            finally
            {
                concurrentDictionary.TryRemove(concurrentRoute.GetHashCode(), out tokenSource);
            }
        }

        private Result TryCancelTask(ConcurrentRoute concurrentRoute)
        {
            CancellationTokenSource tokenSource;
            try
            {
                concurrentDictionary.TryGetValue(concurrentRoute.GetHashCode(), out tokenSource);

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
                concurrentDictionary.TryRemove(concurrentRoute.GetHashCode(), out tokenSource);
            }
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
