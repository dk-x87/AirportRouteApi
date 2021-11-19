namespace AirportRouteApi.Messages
{
    public static class ErrorMessages
    {
        public static readonly string ExceptionError = "Something went wrong. Request could not be completed";
        public static readonly string EmptyCodes = "Both airport's codes should be 3 or 4 character long";
        public static readonly string NotValidSourceAirportCode = "Airport with the presented source airport code does not exist";
        public static readonly string NotValidSourceDestinationCode = "Airport with the presented destination airport code does not exist";
        public static readonly string HandlingProcessNotFound = "Handling process not found";
        public static readonly string ConcurrentRequestLimitExceeded = "Concurrent request limit exceeded";
    }
}
