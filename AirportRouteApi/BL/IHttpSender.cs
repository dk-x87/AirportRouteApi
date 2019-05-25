using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public interface IHttpSender
    {
        Task<HttpResponseMessage> GetDataForRoute(string from, CancellationToken ct);

        Task<HttpResponseMessage> GetDataForAirport(string alias, CancellationToken ct);

        Task<HttpResponseMessage> GetDataForAirline(string alias, CancellationToken ct);

    }
}
