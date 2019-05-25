using System;
using System.Threading.Tasks;
using AirportRouteApi.BL;
using AirportRouteApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AirportRouteApi.Controllers
{
    [Route("api/[controller]")]
    public class RoutesController : Controller
    {
        public RoutesController(IRequestsManager reqManager, ILogger<RoutesController> log)
        {
            requestsManager = reqManager;
            logger = log;
        }

        private readonly IRequestsManager requestsManager;
        private readonly ILogger logger;
        private string userAgent;
        private string remoteAddress;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            userAgent = HttpContext.Request.Headers["user-agent"];
            remoteAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            base.OnActionExecuting(context);
        }

        [Route("GetRoute")]
        [HttpGet]
        public async Task<Result> GetRoute(string from, string to, int maxTransferCount = 0)
        {
            try
            {
                return await requestsManager.TrySetTask(from.ToUpper(), to.ToUpper(), maxTransferCount, userAgent, remoteAddress);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ExceptionError);
                return new Result() { Error = ErrorMessages.ExceptionError };
            }
        }

        [Route("StopRouteProcessing")]
        [HttpGet]
        public Result StopRouteProcessing(string from, string to)
        {
            try
            {
                return requestsManager.CancelTask(from, to, userAgent, remoteAddress);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ExceptionError);
                return new Result() { Error = ErrorMessages.ExceptionError };
            }
        }

        [Route("error")]
        public Result OnError()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
                logger.LogError(ex.Error, ErrorMessages.ExceptionError);
                return new Result() { Error = ErrorMessages.ExceptionError };
            }
            else return null;
        }

    }
}
