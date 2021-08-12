namespace AirportRouteApi.BL
{
    public class RouteParams
    {
        public string RouteUri  { get; set; }
        public string AirportUri { get; set; }
        public string AirlineUri { get; set; }
        public int MaxRequestCount { get; set; }
    }
}
