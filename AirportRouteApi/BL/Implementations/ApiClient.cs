using AirportRouteApi.Models;
using Newtonsoft.Json;
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
            await TrySearchAsync(from, to, maxTransferCount, ct);
            return routes.Count > 0 && routes[0].SrcAirport == from && routes[routes.Count - 1].DestAirport == to ? routes : null;
        }

        public async Task<bool> IsValidAirport(string alias, CancellationToken ct)
        {
            var airports = await sender.GetDataForAirport(alias, ct);
            return airports != null && airports.Count > 0 && airports.Find(x => x.Alias.Equals(alias)) != null;
        }


        List<Route> routes = new List<Route>();
        private async Task<Route> TrySearchAsync(string from, string to, int maxTransferCount, CancellationToken ct)
        {
            var listRoutes = await sender.GetDataForRoute(from, ct);
            var choosed = listRoutes.Find(x => x.SrcAirport == from && x.DestAirport == to);
            var route = (choosed != null && await IsActiveAirline(choosed.Airline, ct)) ? choosed : null;
            if (route != null)
            {
                routes.Add(route);
            }
            else 
            {
                if (routes.Count < maxTransferCount)
                {
                    int count = 0;
                    while (count <= listRoutes.Count - 1 && route == null)
                    {
                        if (listRoutes[count].DestAirport != to && await IsActiveAirline(listRoutes[count].Airline, ct))
                        {
                            routes.Add(listRoutes[count]);
                            route = await TrySearchAsync(listRoutes[count].DestAirport, to, maxTransferCount, ct);
                        }
                        count++;
                    }
                }
                else
                {
                    routes.RemoveAt(routes.Count - 1);
                }
            }
            return route;
        }

        private async Task<bool> IsActiveAirline(string alias, CancellationToken ct)
        {
            var airlines = await sender.GetDataForAirline(alias, ct);
            return airlines != null && airlines.Count > 0 && airlines.Find(x => x.Active) != null;
        }

    }
}
