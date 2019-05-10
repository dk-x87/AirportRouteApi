namespace AirportRouteApi.Models
{
    public static class RouteHelper
    {
        public static int GetHashCode(string srcAirport, string destAirport, string userAgent, string remoteAddress)
        {
            int hashcode = srcAirport.GetHashCode();
            hashcode = 31 * hashcode + destAirport.GetHashCode();
            if (!string.IsNullOrEmpty(userAgent))
                hashcode = 31 * hashcode + userAgent.GetHashCode();
            if (!string.IsNullOrEmpty(remoteAddress))
                hashcode = 31 * hashcode + remoteAddress.GetHashCode();
            return hashcode;
        }
    }
}
