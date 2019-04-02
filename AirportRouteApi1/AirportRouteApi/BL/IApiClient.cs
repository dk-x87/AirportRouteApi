using AirportRouteApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public interface IApiClient
    {
        Task<Route> GetRoutesByAirports(string from, string to, CancellationToken ct);

        Task<bool> IsValidAirport(string alias, CancellationToken ct);
    }
}
