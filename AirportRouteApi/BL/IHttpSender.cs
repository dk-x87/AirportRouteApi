using AirportRouteApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public interface IHttpSender
    {
        Task<List<Route>> GetDataForRoute(string from, CancellationToken ct);

        Task<List<Airport>> GetDataForAirport(string alias, CancellationToken ct);

        Task<List<Airline>> GetDataForAirline(string alias, CancellationToken ct);

    }
}
