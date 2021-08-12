using System;
using System.Net;

namespace AirportRouteApi.Models
{
    public class Error
    {
        public HttpStatusCode Code { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        private Error() { }

        public static Error GetConflictErrorResult(string message)
        {
            return new Error()
            {
                Code = HttpStatusCode.Conflict,
                Message = message
            };
        }

        public static Error GetServiceUnavailableResult(string message)
        {
            return new Error()
            {
                Code = HttpStatusCode.ServiceUnavailable,
                Message = message
            };
        }

        public static Error GetInnerErrorResult(Exception exception)
        {
            return new Error()
            {
                Code = HttpStatusCode.InternalServerError,
                Exception = exception
            };
        }
    }
}
