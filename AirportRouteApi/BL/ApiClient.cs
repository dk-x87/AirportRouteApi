using AirportRouteApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public class ApiClient : IApiClient
    {
        public ApiClient(IHttpSender sender, int maxTransferCount)
        {
            this.sender = sender;
            this.maxTransferCountFromSettings = maxTransferCount;
        }

        private readonly IHttpSender sender;
        private readonly int maxTransferCountFromSettings;
        private int maxTransferCount;

        public async Task<List<Route>> GetRoutesByAirports(string from, string to, int maxTransferCount, CancellationToken ct)
        {
            this.maxTransferCount = maxTransferCount > 0 ? maxTransferCount : maxTransferCountFromSettings;
            await TrySearchAsync(from, to, ct);
            return routes.Count > 0 && routes[0].SrcAirport == from && routes[routes.Count - 1].DestAirport == to ? routes : null;
        }

        public async Task<bool> IsValidAirport(string alias, CancellationToken ct)
        {
            HttpResponseMessage response = await sender.GetDataForAirport(alias, ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);
            return airports != null && airports.Count > 0 && airports.Find(x => x.Alias.Equals(alias)) != null;
        }


        List<Route> routes = new List<Route>();
        private async Task<Route> TrySearchAsync(string from, string to, CancellationToken ct)
        {
            var response = await sender.GetDataForRoute(from, ct).Result.Content.ReadAsStringAsync();
            var listDeserialized = JsonConvert.DeserializeObject<List<Route>>(response);
            var deserialized = listDeserialized.Find(x => x.SrcAirport == from && x.DestAirport == to);
            var route = (deserialized != null && await IsActiveAirline(deserialized.Airline, ct)) ? deserialized : null;
            if (route != null)
            {
                routes.Add(route);
            }
            else 
            {
                if (routes.Count < maxTransferCount)
                {
                    int count = 0;
                    while (count <= listDeserialized.Count - 1 && route == null)
                    {
                        if (listDeserialized[count].DestAirport != to && await IsActiveAirline(listDeserialized[count].Airline, ct))
                        {
                            routes.Add(listDeserialized[count]);
                            route = await TrySearchAsync(listDeserialized[count].DestAirport, to, ct);
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
            HttpClient client = new HttpClient();
            var response = await sender.GetDataForAirline(alias, ct).Result.Content.ReadAsStringAsync();
            List<Airline> airlines = JsonConvert.DeserializeObject<List<Airline>>(response);
            return airlines != null && airlines.Count > 0 && airlines.Find(x => x.Active) != null;
        }

    }
}
