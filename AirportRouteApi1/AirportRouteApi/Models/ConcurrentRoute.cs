using System.Threading;

namespace AirportRouteApi.Models
{
    public class ConcurrentRoute
    {
        public string SrcAirport { get; set; }

        public string DestAirport { get; set; }

        public string UserAgent { get; set; }

        public string RemoteAddress { get; set; }

        public CancellationTokenSource TokenSource { get ; set;}

        public override int GetHashCode()
        {
            int hashcode = SrcAirport.GetHashCode();
            hashcode = 31 * hashcode + DestAirport.GetHashCode();
            if (!string.IsNullOrEmpty(UserAgent))
                hashcode = 31 * hashcode + UserAgent.GetHashCode();
            if (!string.IsNullOrEmpty(RemoteAddress))
                hashcode = 31 * hashcode + RemoteAddress.GetHashCode();
            return hashcode;
        }
    }
}
