using AirportRouteApi.Models;
using System;
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

        public async Task<List<Route>> GetRoutesByAirports(string from, string to, CancellationToken ct)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", routeUri, from), ct);
                var jsonString = await response.Content.ReadAsStringAsync();
                var routes = JsonConvert.DeserializeObject<List<Route>>(jsonString).FindAll(x =>
                    {
                        return x.SrcAirport == from && x.DestAirport == to && IsActiveAirline(x.Airline, ct).Result;
                    });
                return routes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> IsValidAirport(string alias, CancellationToken ct)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", airportUri, alias), ct);
                var jsonString = await response.Content.ReadAsStringAsync();
                List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);
                return airports != null && airports.Count > 0 && airports.Find(x => x.Alias.Equals(alias)) != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> IsActiveAirline(string alias, CancellationToken ct)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", airlineUri, alias), ct);
                var jsonString = await response.Content.ReadAsStringAsync();
                List<Airline> airlines = JsonConvert.DeserializeObject<List<Airline>>(jsonString);
                return airlines != null && airlines.Count > 0 && airlines.Find(x => x.Active) != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
