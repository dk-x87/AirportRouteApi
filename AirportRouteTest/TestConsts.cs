namespace AirportRouteTest
{
    public static class TestConsts
    {
        public static string RouteUri = "https://homework.appulate.com/api/Route/outgoing?airport=";
        public static string AirportUri = "https://homework.appulate.com/api/Airport/search?pattern=";
        public static string AirlineUri = "https://homework.appulate.com/api/Airline/";
        public static int MaxTransferCount = 3;
        public static int MaxRequestCount = 3;

        public static string ExistRouteSrcAirport = "VOZ";
        public static string ExistRouteDestAirport = "VKO";
        public static string ExistRouteAirline = "UR";

        public static string NotExistRouteSrcAirport = "DAA";
        public static string NotExistRouteDestAirport = "BIA";
        public static string NotExistRouteAirline = "HQ";

        public static string MultipleTransferRouteSrcAirport1 = "ACE";
        public static string MultipleTransferRouteSrcAirport2 = "BHX";
        public static string MultipleTransferRouteDestAirport = "FUE";
        public static string MultipleTransferRouteAirline1 = "HQ";
        public static string MultipleTransferRouteAirline2 = "TO";

        public static string MultipleTransferRouteMaxTransferCountSrcAirport1 = "NSI";
        public static string MultipleTransferRouteMaxTransferCountSrcAirport2 = "CDG";
        public static string MultipleTransferRouteMaxTransferCountSrcAirport3 = "DLA";
        public static string MultipleTransferRouteMaxTransferCountSrcAirport4 = "GOU";
        public static string MultipleTransferRouteMaxTransferCountDestAirport = "CAI";
        public static string MultipleTransferRouteMaxTransferCountAirline1 = "QC";

        public static string NotExistAirport = "DAQ";
    }
}
