using AirportRouteApi;
using AirportRouteApi.BL;
using AirportRouteApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace AirportRouteTest
{
    [TestFixture]
    public class RoutesControllerIntegrationTests
    {
        private RoutesController controller;

        [SetUp]
        public void Setup()
        {
            IHttpSender sender = new HttpSender(TestConsts.RouteUri, TestConsts.AirportUri, TestConsts.AirlineUri, TestConsts.MaxRequestCount);
            IApiClient apiClient = new ApiClient(sender, TestConsts.MaxTransferCount);
            RequestsManager reqManager = new RequestsManager(apiClient);
            ILogger<RoutesController> log = new LoggerFactory().CreateLogger<RoutesController>();

            controller = new RoutesController(reqManager, log);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Request.Headers["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
            controller.HttpContext.Connection.RemoteIpAddress = new System.Net.IPAddress(1);
        }

        [Test]
        public void GetRouteEmptyFromToCodesTest()
        {
            var response = controller.GetRoute(string.Empty, string.Empty);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.EmptyCodes);
        }

        [Test]
        public void GetRouteEmptyToCodeTest()
        {
            var response = controller.GetRoute(TestConsts.ExistRouteSrcAirport, string.Empty);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.EmptyCodes);
        }

        [Test]
        public void GetRouteEmptyFromCodeTest()
        {
            var response = controller.GetRoute(string.Empty, TestConsts.ExistRouteSrcAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.EmptyCodes);
        }

        [Test]
        public void GetRouteNotExistFromAirportTest()
        {
            var response = controller.GetRoute(TestConsts.ExistRouteSrcAirport, TestConsts.NotExistAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.NotValidSourceDestinationCode);
        }

        [Test]
        public void GetRouteNotExistToAirportTest()
        {
            var response = controller.GetRoute(TestConsts.NotExistAirport, TestConsts.ExistRouteSrcAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.NotValidSourceAirportCode);
        }

        [Test]
        public void GetRouteExistAirportTest()
        {
            var response = controller.GetRoute(TestConsts.ExistRouteSrcAirport, TestConsts.ExistRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Routes);
            Assert.AreEqual(result.Routes[0].SrcAirport, TestConsts.ExistRouteSrcAirport);
            Assert.AreEqual(result.Routes[result.Routes.Count - 1].DestAirport, TestConsts.ExistRouteDestAirport);
        }

        [Test]
        public void GetRouteNotExistAirportTest()
        { 
            var response = controller.GetRoute(TestConsts.NotExistRouteSrcAirport, TestConsts.NotExistRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNull(result.Routes);
        }

        [Test]
        public void GetRouteMultipleTransferTest()
        {
            var response = controller.GetRoute(TestConsts.MultipleTransferRouteSrcAirport1, TestConsts.MultipleTransferRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Routes);
            Assert.AreEqual(TestConsts.MultipleTransferRouteSrcAirport1, result.Routes[0].SrcAirport);
            Assert.AreEqual(TestConsts.MultipleTransferRouteDestAirport, result.Routes[result.Routes.Count - 1].DestAirport);
            Assert.True(result.Routes.Count > 0);
        }

        [Test]
        public void GetRouteMultipleTransferWithMaxTransferTest()
        {
            var response = controller.GetRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport1, TestConsts.MultipleTransferRouteMaxTransferCountDestAirport, 1);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNull(result.Routes);
        }

        [Test]
        public void StopRouteProcessingTest()
        {
            controller.GetRoute(TestConsts.ExistRouteSrcAirport, TestConsts.ExistRouteDestAirport);
            var response = controller.StopRouteProcessing(TestConsts.ExistRouteSrcAirport, TestConsts.ExistRouteDestAirport);
            Assert.IsNotNull(response);
            var result = response;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.Message.Equals(ErrorMessages.ProcessWasStoped) || result.Message.Equals(ErrorMessages.HandlingProcessNotFound));
        }
    }
}