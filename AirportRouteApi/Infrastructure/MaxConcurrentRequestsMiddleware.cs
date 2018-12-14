using AirportRouteApi.BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.Tasks;

namespace AirportRouteApi.Infrastructure
{
    public class MaxConcurrentRequestsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly int maxConcurrentRequests;
        private IRequestsManager requestsManager;

        public MaxConcurrentRequestsMiddleware(RequestDelegate nxt, int maxRequests)
        {
            next = nxt;
            maxConcurrentRequests = maxRequests;
        }

        public async Task Invoke(HttpContext context, IRequestsManager reqManager)
        {
            requestsManager = reqManager;
            if (CheckLimitExceeded())
            {
                IHttpResponseFeature responseFeature = context.Features.Get<IHttpResponseFeature>();

                responseFeature.StatusCode = StatusCodes.Status503ServiceUnavailable;
                responseFeature.ReasonPhrase = ErrorMessages.ConcurrentRequestLimitExceeded;
            }
            else
            {
                await next(context);
            }
        }

        private bool CheckLimitExceeded()
        {
            return requestsManager.ProcessingsTasksCount() >= maxConcurrentRequests;
        }
    }
}