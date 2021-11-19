using AirportRouteApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL.Implementations
{
    public class HttpSender : IHttpSender
    {
        public HttpSender(RouteParams routeParams)
        {
            routeUri = routeParams.RouteUri;
            airportUri = routeParams.AirportUri;
            airlineUri = routeParams.AirlineUri;
            maxRequestCount = routeParams.MaxRequestCount;
        }
        private readonly string routeUri;
        private readonly string airportUri;
        private readonly string airlineUri;
        private readonly int maxRequestCount;

        public async Task<List<Route>> GetDataForRoute(string from, CancellationToken ct)
        {
            return await GetData<List<Route>>($"{routeUri}{from}", ct);
        }

        public async Task<List<Airport>> GetDataForAirport(string alias, CancellationToken ct)
        {
            return await GetData<List<Airport>>($"{airportUri}{alias}", ct);
        }

        public async Task<List<Airline>> GetDataForAirline(string alias, CancellationToken ct)
        {
            return await GetData<List<Airline>>($"{airlineUri}{alias}", ct);
        }


        private async Task<T> GetData<T>(string url, CancellationToken ct)
        {
            int count = 0;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            while ((response == null || !response.IsSuccessStatusCode) && count++ < maxRequestCount)
            {
                response = await client.GetAsync(url, ct);
            }
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
