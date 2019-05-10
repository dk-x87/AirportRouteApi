using AirportRouteApi.Models;
using System.Threading.Tasks;

namespace AirportRouteApi.BL
{
    public interface IRequestsManager
    {
        Task<Result> TrySetTask(string from, string to, int maxTransferCount, string userAgent, string remoteAddress);

        Result CancelTask(string from, string to, string userAgent, string remoteAddress);

        int ProcessingsTasksCount();
    }
}
