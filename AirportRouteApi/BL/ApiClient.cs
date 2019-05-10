using AirportRouteApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace AirportRouteApi.BL
{
    public class ApiClient : IApiClient
    {
        public ApiClient(string routeUri, string airportUri, string airlineUri, int maxTransferCount, int maxRequestCount)
        {
            this.routeUri = routeUri;
            this.airportUri = airportUri;
            this.airlineUri = airlineUri;
            this.maxTransferCountFromSettings = maxTransferCount;
            this.maxRequestCount = maxRequestCount;
        }
   
        private readonly string routeUri;
        private readonly string airportUri;
        private readonly string airlineUri;
        private readonly int maxTransferCountFromSettings;
        private readonly int maxRequestCount;
        private int maxTransferCount;

        public async Task<List<Route>> GetRoutesByAirports(string from, string to, int maxTransferCount, CancellationToken ct)
        {
            this.maxTransferCount = maxTransferCount > 0 ? maxTransferCount : maxTransferCountFromSettings;
            await TrySearchAsync(from, to, ct);
            return routes;
        }

        public async Task<bool> IsValidAirport(string alias, CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", airportUri, alias), ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);
            return airports != null && airports.Count > 0 && airports.Find(x => x.Alias.Equals(alias)) != null;
        }

        List<Route> routes = new List<Route>();
        private async Task<Route> TrySearchAsync(string from, string to, CancellationToken ct)
        {
            var response = await GetData(string.Format("{0}{1}", routeUri, from), ct).Result.Content.ReadAsStringAsync();
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
            var response = await GetData(string.Format("{0}{1}", airlineUri, alias), ct).Result.Content.ReadAsStringAsync();
            List<Airline> airlines = JsonConvert.DeserializeObject<List<Airline>>(response);
            return airlines != null && airlines.Count > 0 && airlines.Find(x => x.Active) != null;
        }

        private async Task<HttpResponseMessage> GetData(string url, CancellationToken ct)
        {
            int count = 0;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            while ((response == null || !response.IsSuccessStatusCode) && count < maxRequestCount)
            {
                response = await client.GetAsync(url, ct);
            }
            return response;
        }
    }
}
