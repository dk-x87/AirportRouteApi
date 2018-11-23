using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public interface IRequestsManager
    {
        Task<string> TrySetTask(string from, string to, string userAgent, string remoteAddress);

        string CancelTask(string from, string to, string userAgent, string remoteAddress);
    }
}
