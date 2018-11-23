using System;
using System.Threading.Tasks;
using AirportRouteApi.BL;
using AirportRouteApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

        [Route("GetRoute")]
        [HttpGet]
        public async Task<string> GetRoute(string from, string to)
        {
            try
            {
                string userAgent = HttpContext.Request.Headers["user-agent"];
                string remoteAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                return await requestsManager.TrySetTask(from, to, userAgent, remoteAddress);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ExceptionError);
                return JsonConvert.SerializeObject(new Result() { Error = ErrorMessages.ExceptionError });
            }
        }

        [Route("StopRouteProcessing")]
        [HttpGet]
        public string StopRouteProcessing(string from, string to)
        {
            try
            {
                string userAgent = HttpContext.Request.Headers["user-agent"];
                string remoteAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                return requestsManager.CancelTask(from, to, userAgent, remoteAddress);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.ExceptionError);
                return JsonConvert.SerializeObject(new Result() { Error = ErrorMessages.ExceptionError });
            }
        }

        [Route("error")]
        public string OnError()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
                logger.LogError(ex.Error, ErrorMessages.ExceptionError);
                return JsonConvert.SerializeObject(new Result() { Error = ErrorMessages.ExceptionError });
            }
            else return string.Empty;
        }

    }
}
