using AirportRouteApi;
using AirportRouteApi.BL;
using AirportRouteApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class RoutesControllerIntegrationTests
    {
        private RoutesController controller;
        private string routeUri = "https://homework.appulate.com/api/Route/outgoing?airport=";
        private string airportUri = "https://homework.appulate.com/api/Airport/search?pattern=";
        private string airlineUri = "https://homework.appulate.com/api/Airline/";
        private int maxTransferCount = 3;
        private int maxRequestCount = 3;
        private string existRouteSrcAirport = "VOZ";
        private string existRouteDestAirport = "VKO";
        private string notExistRouteSrcAirport = "DAA";
        private string notExistRouteDestAirport = "BIA";
        private string multipleTransferRouteSrcAirport = "ACE";
        private string multipleTransferRouteDestAirport = "FUE";
        private string multipleTransferRouteMaxTransferCountSrcAirport = "EVN";
        private string multipleTransferRouteMaxTransferCountDestAirport = "CAI";
        private string notExistAirport = "DAQ";

        [SetUp]
        public void Setup()
        {
            RequestsManager reqManager = new RequestsManager(new ApiClient(routeUri, airportUri, airlineUri, maxTransferCount, maxRequestCount) { });
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
            var response = controller.GetRoute(existRouteSrcAirport, string.Empty);
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
            var response = controller.GetRoute(string.Empty, existRouteSrcAirport);
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
            var response = controller.GetRoute(existRouteSrcAirport, notExistAirport);
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
            var response = controller.GetRoute(notExistAirport, existRouteSrcAirport);
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
            var response = controller.GetRoute(existRouteSrcAirport, existRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Routes);
            Assert.AreEqual(result.Routes[0].SrcAirport, existRouteSrcAirport);
            Assert.AreEqual(result.Routes[result.Routes.Count - 1].DestAirport, existRouteDestAirport);
        }

        [Test]
        public void GetRouteNotExistAirportTest()
        { 
            var response = controller.GetRoute(notExistRouteSrcAirport, notExistRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsEmpty(result.Routes);
        }

        [Test]
        public void GetRouteMultipleTransferTest()
        {
            var response = controller.GetRoute(multipleTransferRouteSrcAirport, multipleTransferRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Routes);
            Assert.AreEqual(multipleTransferRouteSrcAirport, result.Routes[0].SrcAirport);
            Assert.AreEqual(multipleTransferRouteDestAirport, result.Routes[result.Routes.Count - 1].DestAirport);
            Assert.True(result.Routes.Count > 0);
        }

        [Test]
        public void GetRouteMultipleTransferWithMaxTransferTest()
        {
            var response = controller.GetRoute(multipleTransferRouteMaxTransferCountSrcAirport, multipleTransferRouteMaxTransferCountDestAirport, 1);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = response.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsEmpty(result.Routes);
        }

        [Test]
        public void StopRouteProcessingTest()
        {
            controller.GetRoute(existRouteSrcAirport, existRouteDestAirport);
            var response = controller.StopRouteProcessing(existRouteSrcAirport, existRouteDestAirport);
            Assert.IsNotNull(response);
            var result = response;
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.Message.Equals(ErrorMessages.ProcessWasStoped) || result.Message.Equals(ErrorMessages.HandlingProcessNotFound));
        }
    }
}