using System.Collections.Generic;

namespace AirportRouteApi.Models
{
    public class Result
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public List<Route> Routes { get; set; }
    }
}
