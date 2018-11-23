using System.Threading;

namespace AirportRouteApi.Models
{
    public class ConcurrentRoute
    {
        public string SrcAirport { get; set; }

        public string DestAirport { get; set; }

        public string UserAgent { get; set; }

        public string RemoteAddress { get; set; }

        public CancellationToken Token { get ; set;}
    }
}
