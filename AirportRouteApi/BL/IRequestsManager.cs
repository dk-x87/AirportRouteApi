using AirportRouteApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public interface IRequestsManager
    {
        Task<Responce<List<Route>>> TrySetTask(string from, string to, int maxTransferCount, string userAgent, string remoteAddress);

        Responce<string> CancelTask(string from, string to, string userAgent, string remoteAddress);
    }
}
