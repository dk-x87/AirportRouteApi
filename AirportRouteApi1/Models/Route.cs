namespace AirportRouteApi.Models
{
    public class Route
    {
        public string Airline { get; set; }

        public string SrcAirport { get; set; }

        public string DestAirport { get; set; }

        public int TransferCount { get; set; }
    }
}