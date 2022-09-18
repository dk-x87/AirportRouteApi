using AirportRouteApi.BL;
using AirportRouteApi.BL.Implementations;
using AirportRouteApi.Controllers;
using AirportRouteApi.Messages;
using AirportRouteApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AirportRouteTest
{
    [TestFixture]
    public class RoutesControllerIntegrationTests
    {
        private RoutesController controller;

        [SetUp]
        public void Setup()
        {
            IHttpSender sender = new HttpSender(new RouteParams()
            {
                RouteUri = TestConsts.RouteUri,
                AirportUri = TestConsts.AirportUri,
                AirlineUri = TestConsts.AirlineUri,
                MaxRequestCount = TestConsts.MaxRequestCount
            });
            IApiClient apiClient = new ApiClient(sender);
            ILogger<RequestsManager> log = new LoggerFactory().CreateLogger<RequestsManager>();
            RequestParams requestParams = new RequestParams()
            {
                MaxConcurrentRequests = TestConsts.MaxRequestCount,
                MaxTransferCount = TestConsts.MaxTransferCount
            };
            RequestsManager reqManager = new RequestsManager(apiClient, requestParams, log);

            controller = new RoutesController(reqManager);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Request.Headers["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
            controller.HttpContext.Connection.RemoteIpAddress = new System.Net.IPAddress(1);
        }


        [Test]
        public async Task  GetRouteEmptyFromToCodesTest()
        {
            var response = await controller.GetRoute(string.Empty, string.Empty);
            Assert.IsNotNull(response);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.AreEqual(ErrorMessages.EmptyCodes, result.Value);
        }

        [Test]
        public async Task GetRouteEmptyToCodeTest()
        {
            var response = await controller.GetRoute(TestConsts.ExistRouteSrcAirport, string.Empty);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.AreEqual(ErrorMessages.EmptyCodes, result.Value);
        }

        [Test]
        public async Task GetRouteEmptyFromCodeTest()
        {
            var response = await controller.GetRoute(string.Empty, TestConsts.ExistRouteSrcAirport);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.AreEqual(result.Value, ErrorMessages.EmptyCodes);
        }

        [Test]
        public async Task GetRouteNotExistFromAirportTest()
        {
            var response = await controller.GetRoute(TestConsts.ExistRouteSrcAirport, TestConsts.NotExistAirport);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.AreEqual(result.Value, ErrorMessages.NotValidSourceDestinationCode);
        }

        [Test]
        public async Task GetRouteNotExistToAirportTest()
        {
            var response = await controller.GetRoute(TestConsts.NotExistAirport, TestConsts.ExistRouteSrcAirport);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.AreEqual(result.Value, ErrorMessages.NotValidSourceAirportCode);
        }

        [Test]
        public async Task GetRouteExistAirportTest()
        {
            var response = await controller.GetRoute(TestConsts.ExistRouteSrcAirport, TestConsts.ExistRouteDestAirport);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Value);
            var routes = (List<Route>)result.Value;
            Assert.AreEqual(routes[0].SrcAirport, TestConsts.ExistRouteSrcAirport);
            Assert.AreEqual(routes[routes.Count - 1].DestAirport, TestConsts.ExistRouteDestAirport);
        }

        [Test]
        public async Task GetRouteNotExistAirportTest()
        {
            var response = await controller.GetRoute(TestConsts.NotExistRouteSrcAirport, TestConsts.NotExistRouteDestAirport);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNull(result.Value);
        }

        [Test]
        public async Task GetRouteMultipleTransferTest()
        {
            var response = await controller.GetRoute(TestConsts.MultipleTransferRouteSrcAirport1, TestConsts.MultipleTransferRouteDestAirport);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Value);
            var routes = (List<Route>)result.Value;
            Assert.AreEqual(TestConsts.MultipleTransferRouteSrcAirport1, routes[0].SrcAirport);
            Assert.AreEqual(TestConsts.MultipleTransferRouteDestAirport, routes[routes.Count - 1].DestAirport);
            Assert.True(routes.Count > 0);
        }

        [Test]
        public async Task GetRouteMultipleTransferWithMaxTransferTest()
        {
            var response = await controller.GetRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport1, TestConsts.MultipleTransferRouteMaxTransferCountDestAirport, 1);
            var result = (ObjectResult)response;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNull(result.Value);
        }

        [Test]
        public void StopRouteProcessingTest()
        {
            controller.GetRoute(TestConsts.ExistRouteSrcAirport, TestConsts.ExistRouteDestAirport);
            var response = controller.StopRouteProcessing(TestConsts.ExistRouteSrcAirport, TestConsts.ExistRouteDestAirport);
            var result = (ObjectResult)response;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.Equals(SuccessMessages.ProcessWasStoped) || result.Value.Equals(ErrorMessages.HandlingProcessNotFound));
        }

    }
}