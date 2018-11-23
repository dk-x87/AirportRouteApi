using AirportRouteApi;
using AirportRouteApi.BL;
using AirportRouteApi.Controllers;
using AirportRouteApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class RoutesControllerTests
    {
        private RoutesController controller;
        private string routeUri = "https://homework.appulate.com/api/Route/outgoing?airport=";
        private string airportUri = "https://homework.appulate.com/api/Airport/search?pattern=";
        private string airlineUri = "https://homework.appulate.com/api/Airline/";
        private string existRouteSrcAirport = "VOZ";
        private string existRouteDestAirport = "VKO";
        private string notExistRouteSrcAirport = "DAA";
        private string notExistRouteDestAirport = "BIA";
        private string notActiveAirlineRouteSrcAirport = "ACE";
        private string notActiveAirlineRouteDestAirport = "FUE";
        private string notExistAirport = "DAQ";

        [SetUp]
        public void Setup()
        {
            RequestsManager reqManager = new RequestsManager(new ApiClient(routeUri, airportUri, airlineUri) { });
            ILogger<RoutesController> log = new LoggerFactory().CreateLogger<RoutesController>();

            controller = new RoutesController(reqManager, log);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Request.Headers["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
            controller.HttpContext.Connection.RemoteIpAddress = new System.Net.IPAddress(1);
        }

        [Test]
        public void GetRouteEmptyCodesTest()
        {
            var response = controller.GetRoute(string.Empty, string.Empty);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            var result = JsonConvert.DeserializeObject<Result>(response.Result);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.EmptyCodes);

            response = controller.GetRoute(existRouteSrcAirport, string.Empty);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            result = JsonConvert.DeserializeObject<Result>(response.Result);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.EmptyCodes);

            response = controller.GetRoute(string.Empty, existRouteSrcAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            result = JsonConvert.DeserializeObject<Result>(response.Result);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.EmptyCodes);
        }

        [Test]
        public void GetRouteNotExistAirportTest()
        {
            var response = controller.GetRoute(existRouteSrcAirport, notExistAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            var result = JsonConvert.DeserializeObject<Result>(response.Result);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Routes);
            Assert.AreEqual(result.Error, ErrorMessages.NotValidSourceDestinationCode);

            response = controller.GetRoute(notExistAirport, existRouteSrcAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            Assert.IsNotNull(response.Result);
            result = JsonConvert.DeserializeObject<Result>(response.Result);
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
            var result = JsonConvert.DeserializeObject<Result>(response.Result);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Routes);
            Assert.NotZero(result.Routes.Count);
            Assert.AreEqual(result.Routes[0].SrcAirport, existRouteSrcAirport);
            Assert.AreEqual(result.Routes[0].DestAirport, existRouteDestAirport);

            response = controller.GetRoute(notExistRouteSrcAirport, notExistRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            result = JsonConvert.DeserializeObject<Result>(response.Result);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Routes);
            Assert.Zero(result.Routes.Count);
        }

        [Test]
        public void GetRouteNotActiveAirlineTest()
        {
            var response = controller.GetRoute(notActiveAirlineRouteSrcAirport, notActiveAirlineRouteDestAirport);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
            var result = JsonConvert.DeserializeObject<Result>(response.Result);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Routes);
            Assert.Zero(result.Routes.Count);
        }

        [Test]
        public void StopRouteProcessingTest()
        {
            controller.GetRoute(existRouteSrcAirport, existRouteDestAirport);
            var response = controller.StopRouteProcessing(existRouteSrcAirport, existRouteDestAirport);
            Assert.IsNotNull(response);
            var result = JsonConvert.DeserializeObject<Result>(response);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Error);
            Assert.IsTrue(result.Message.Equals(ErrorMessages.ProcessWasStoped) || result.Message.Equals(ErrorMessages.HandlingProcessNotFound));
        }
    }
}