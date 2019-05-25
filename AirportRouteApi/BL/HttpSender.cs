using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public class HttpSender : IHttpSender
    {
        public HttpSender(string routeUri, string airportUri, string airlineUri, int maxRequestCount)
        {
            this.routeUri = routeUri;
            this.airportUri = airportUri;
            this.airlineUri = airlineUri;
            this.maxRequestCount = maxRequestCount;
        }
        private readonly string routeUri;
        private readonly string airportUri;
        private readonly string airlineUri;
        private readonly int maxRequestCount;

        public async Task<HttpResponseMessage> GetDataForRoute(string from, CancellationToken ct)
        {
            string url = $"{routeUri}{from}";
            return await GetData(url, ct);
        }

        public async Task<HttpResponseMessage> GetDataForAirport(string alias, CancellationToken ct)
        {
            string url = $"{airportUri}{alias}";
            return await GetData(url, ct);
        }

        public async Task<HttpResponseMessage> GetDataForAirline(string alias, CancellationToken ct)
        {
            string url = $"{airlineUri}{alias}";
            return await GetData(url, ct);
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
