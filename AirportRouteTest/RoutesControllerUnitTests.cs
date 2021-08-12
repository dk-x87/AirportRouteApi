using AirportRouteApi;
using AirportRouteApi.BL;
using AirportRouteApi.BL.Implementations;
using AirportRouteApi.Controllers;
using AirportRouteApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteTest
{
    [TestFixture]
    public class RoutesControllerUnitTests
    {
        private RoutesController controller;

        private T LoadDataFromJson<T>(string filename)
        {
            string content = string.Empty;
            using (StreamReader r = new StreamReader(filename))
            {
                content = r.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(content);
        }


        private IHttpSender MockIHttpSender()
        {
            var existRouteSrcAirportValid = LoadDataFromJson<List<Airport>>("Jsons/existRouteSrcAirportValid.json");
            var existRouteDestAirportValid = LoadDataFromJson<List<Airport>>("Jsons/existRouteDestAirportValid.json");
            var existRouteSrcAirport = LoadDataFromJson<List<Route>>("Jsons/existRouteSrcAirport.json");
            var existRouteAirline = LoadDataFromJson<List<Airline>>("Jsons/existRouteAirline.json");

            var notExistRouteSrcAirportValid= LoadDataFromJson<List<Airport>>("Jsons/notExistRouteSrcAirportValid.json");
            var notExistRouteDestAirportValid= LoadDataFromJson<List<Airport>>("Jsons/notExistRouteDestAirportValid.json");
            var notExistRouteSrcAirport= LoadDataFromJson<List<Route>>("Jsons/notExistRouteSrcAirport.json");

            var multipleTransferRouteSrcAirport1Valid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteSrcAirport1Valid.json");
            var multipleTransferRouteSrcAirport2Valid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteSrcAirport2Valid.json");
            var multipleTransferRouteDestAirportValid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteDestAirportValid.json");
            var multipleTransferRouteSrcAirport1 = LoadDataFromJson<List<Route>>("Jsons/multipleTransferRouteSrcAirport1.json");
            var multipleTransferRouteSrcAirport2 = LoadDataFromJson<List<Route>>("Jsons/multipleTransferRouteSrcAirport2.json");
            var multipleTransferRouteAirline1 = LoadDataFromJson<List<Airline>>("Jsons/multipleTransferRouteAirline1.json");
            var multipleTransferRouteAirline2 = LoadDataFromJson<List<Airline>>("Jsons/multipleTransferRouteAirline2.json");

            var multipleTransferRouteMaxTransferCountSrcAirport1Valid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport1Valid.json");
            var multipleTransferRouteMaxTransferCountSrcAirport2Valid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport2Valid.json");
            var multipleTransferRouteMaxTransferCountSrcAirport3Valid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport3Valid.json");
            var multipleTransferRouteMaxTransferCountSrcAirport4Valid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport4Valid.json");
            var multipleTransferRouteMaxTransferCountDestAirportValid = LoadDataFromJson<List<Airport>>("Jsons/multipleTransferRouteMaxTransferCountDestAirportValid.json");
            var multipleTransferRouteMaxTransferCountSrcAirport1 = LoadDataFromJson<List<Route>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport1.json");
            var multipleTransferRouteMaxTransferCountSrcAirport2 = LoadDataFromJson<List<Route>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport2.json");
            var multipleTransferRouteMaxTransferCountSrcAirport3 = LoadDataFromJson<List<Route>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport3.json");
            var multipleTransferRouteMaxTransferCountSrcAirport4 = LoadDataFromJson<List<Route>>("Jsons/multipleTransferRouteMaxTransferCountSrcAirport4.json");
            var multipleTransferRouteMaxTransferCountAirline1 = LoadDataFromJson<List<Airline>>("Jsons/multipleTransferRouteMaxTransferCountAirline1.json");

            var notExistAirport = LoadDataFromJson<List<Airport>>("Jsons/notExistAirport.json");

            Mock<IHttpSender> mockContainer = new Mock<IHttpSender>();
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.ExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(existRouteSrcAirportValid));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.ExistRouteDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(existRouteDestAirportValid ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.ExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(existRouteSrcAirport ));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.ExistRouteAirline, It.IsAny<CancellationToken>())).Returns(Task.FromResult(existRouteAirline ));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.NotExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(notExistRouteSrcAirportValid ));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.NotExistRouteDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(notExistRouteDestAirportValid ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.NotExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(notExistRouteSrcAirport ));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteSrcAirport1Valid ));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteSrcAirport2Valid ));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteDestAirportValid ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteSrcAirport1 ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteSrcAirport2 ));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.MultipleTransferRouteAirline1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteAirline1 ));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.MultipleTransferRouteAirline2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteAirline2 ));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport1Valid ));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport2Valid ));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport3, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport3Valid ));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport4, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport4Valid ));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountDestAirportValid ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport1 ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport1 ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport3, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport1 ));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport4, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountSrcAirport1 ));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.MultipleTransferRouteMaxTransferCountAirline1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(multipleTransferRouteMaxTransferCountAirline1 ));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.NotExistAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(notExistAirport));
            return mockContainer.Object;
        }

        [SetUp]
        public void Setup()
        {
            IHttpSender sender = MockIHttpSender();
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
        public async Task GetRouteEmptyFromToCodesTest()
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
            Assert.IsTrue(result.Value.Equals(ErrorMessages.ProcessWasStoped) || result.Value.Equals(ErrorMessages.HandlingProcessNotFound));
        }
    }
}
