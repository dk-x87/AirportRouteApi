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
        public ApiClient(string routeUrl, string airportUrl, string airlineUrl)
        {
            routeUri = routeUrl;
            airportUri = airportUrl;
            airlineUri = airlineUrl;
        }

        private readonly string routeUri;
        private readonly string airportUri;
        private readonly string airlineUri;

        public async Task<Route> GetRoutesByAirports(string from, string to, CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", routeUri, from), ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            var deserializedRoute = JsonConvert.DeserializeObject<List<Route>>(jsonString).Find(x => x.SrcAirport == from && x.DestAirport == to);
            return (deserializedRoute != null && await IsActiveAirline(deserializedRoute.Airline, ct)) ? deserializedRoute : null;
        }

        public async Task<bool> IsValidAirport(string alias, CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", airportUri, alias), ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);
            return airports != null && airports.Count > 0 && airports.Find(x => x.Alias.Equals(alias)) != null;
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
