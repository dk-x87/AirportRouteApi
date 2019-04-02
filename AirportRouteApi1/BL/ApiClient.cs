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
        public ApiClient(string routeUrl, string airportUrl, string airlineUrl, int mxTransferCount)
        {
            routeUri = routeUrl;
            airportUri = airportUrl;
            airlineUri = airlineUrl;
            maxTransferCount = mxTransferCount;
        }

        private readonly string routeUri;
        private readonly string airportUri;
        private readonly string airlineUri;
        private readonly int maxTransferCount;

        public async Task<Route> GetRoutesByAirports(string from, string to, CancellationToken ct)
        {
            var route = await TrySearch(from, to, ct, 0);
            if (route != null)
            {
                route.SrcAirport = from;
            }
            return route;
        }

        public async Task<bool> IsValidAirport(string alias, CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", airportUri, alias), ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);
            return airports != null && airports.Count > 0 && airports.Find(x => x.Alias.Equals(alias)) != null;
        }

        private async Task<Route> TrySearch(string from, string to, CancellationToken ct, int attempt)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(string.Format("{0}{1}", routeUri, from), ct).Result.Content.ReadAsStringAsync();
            var listDeserialized = JsonConvert.DeserializeObject<List<Route>>(response);
            var deserialized = listDeserialized.Find(x => x.SrcAirport == from && x.DestAirport == to);
            var route = (deserialized != null && await IsActiveAirline(deserialized.Airline, ct)) ? deserialized : null;
            if (route != null)
            {
                route.TransferCount = attempt;
            }
            else if (attempt < maxTransferCount)
            {
                int count = 0;
                while (count <= listDeserialized.Count - 1 && route == null)
                {
                    route = await TrySearch(listDeserialized[count].DestAirport, to, ct, attempt + 1);
                    count++;
                }
            }
            return route;
        }

        private async Task<bool> IsActiveAirline(string alias, CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", airlineUri, alias), ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            List<Airline> airlines = JsonConvert.DeserializeObject<List<Airline>>(jsonString);
            return airlines != null && airlines.Count > 0 && airlines.Find(x => x.Active) != null;
        }
    }
}
