using AirportRouteApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL.Implementations
{
    public class ApiClient : IApiClient
    {
        public ApiClient(IHttpSender sender)
        {
            this.sender = sender;
        }

        private readonly IHttpSender sender;

        public async Task<List<Route>> GetRoutesByAirports(string from, string to, int maxTransferCount, CancellationToken ct)
        {
            var routes = await TrySearchAsync(from, to, maxTransferCount, ct);
            return routes.Count > 0 && routes[0].SrcAirport == from && routes[routes.Count - 1].DestAirport == to ? routes : null;
        }

        public async Task<bool> IsValidAirport(string alias, CancellationToken ct)
        {
            var airports = await sender.GetDataForAirport(alias, ct);
            return airports != null && airports.Count > 0 && airports.Find(x => x.Alias.Equals(alias)) != null;
        }


        private async Task<bool> IsActiveAirline(string alias, CancellationToken ct)
        {
            var airlines = await sender.GetDataForAirline(alias, ct);
            return airlines != null && airlines.Count > 0 && airlines.Find(x => x.Active) != null;
        }

        private async Task<List<Route>> TrySearchAsync(string from, string to, int maxTransferCount, CancellationToken ct)
        {
            List<Route> routes = new List<Route>();
            var routesFromSource = await sender.GetDataForRoute(from, ct);
            var sourceToDest = routesFromSource.Find(x => x.SrcAirport == from && x.DestAirport == to);
            if (sourceToDest != null
                && await IsValidAirport(sourceToDest.DestAirport, ct)
                && await IsActiveAirline(sourceToDest.Airline, ct))
            {
                routes.Add(sourceToDest);
            }
            else
            {
                if (routes.Count < maxTransferCount)
                {
                    bool detected = false;
                    int count = 0;
                    while (count <= routesFromSource.Count - 1 && !detected)
                    {
                        if (await IsValidAirport(routesFromSource[count].DestAirport, ct) && await IsActiveAirline(routesFromSource[count].Airline, ct))
                        {
                            var tryRoutes = await TrySearchAsync(routesFromSource[count].DestAirport, to, maxTransferCount - 1, ct);
                            if (tryRoutes.Count > 0)
                            {
                                routes.Add(routesFromSource[count]);
                                routes.AddRange(tryRoutes);
                                detected = true;
                            }
                        }
                        count++;
                    }
                }
            }
            return routes;
        }

    }
}
