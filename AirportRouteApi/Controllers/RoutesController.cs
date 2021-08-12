using System.Threading.Tasks;
using AirportRouteApi.BL;
using AirportRouteApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AirportRouteApi.Controllers
{
    [Route("api/[controller]")]
    public class RoutesController : Controller
    {
        public RoutesController(IRequestsManager reqManager)
        {
            requestsManager = reqManager;
        }

        private readonly IRequestsManager requestsManager;
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
        public async Task<ActionResult> GetRoute(string from, string to, int maxTransferCount = 0)
        {
            return ProcessResponce(await requestsManager.TrySetTask(from, to, maxTransferCount, userAgent, remoteAddress));
        }

        [Route("StopRouteProcessing")]
        [HttpGet]
        public ActionResult StopRouteProcessing(string from, string to)
        {
            return ProcessResponce(requestsManager.CancelTask(from, to, userAgent, remoteAddress));
        }

        private ActionResult ProcessResponce<T>(Responce<T> responce)
        {
            if (responce.Error != null)
            {
                return StatusCode((int)responce.Error.Code, responce.Error.Message ?? responce.Error.Exception.Message);
            }
            else
            {
                return new OkObjectResult(responce.Data);
            }
        }

    }
}
