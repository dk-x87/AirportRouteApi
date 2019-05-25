using AirportRouteApi;
using AirportRouteApi.BL;
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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportRouteTest
{
    [TestFixture]
    public class RoutesControllerUnitTests
    {
        private RoutesController controller;

        private string LoadJson(string filename)
        {
            using (StreamReader r = new StreamReader(filename))
            {
                return r.ReadToEnd();
            }
        }


        private IHttpSender MockIHttpSender()
        {
            HttpContent existRouteSrcAirportValidCntt = new StringContent(LoadJson("Jsons/existRouteSrcAirportValid.json"));
            HttpContent existRouteDestAirportValidCntt = new StringContent(LoadJson("Jsons/existRouteDestAirportValid.json"));
            HttpContent existRouteSrcAirportCntt = new StringContent(LoadJson("Jsons/existRouteSrcAirport.json"));
            HttpContent existRouteAirlineCntt = new StringContent(LoadJson("Jsons/existRouteAirline.json"));

            HttpContent notExistRouteSrcAirportValidCntt = new StringContent(LoadJson("Jsons/notExistRouteSrcAirportValid.json"));
            HttpContent notExistRouteDestAirportValidCntt = new StringContent(LoadJson("Jsons/notExistRouteDestAirportValid.json"));
            HttpContent notExistRouteSrcAirportCntt = new StringContent(LoadJson("Jsons/notExistRouteSrcAirport.json"));

            HttpContent multipleTransferRouteSrcAirport1ValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteSrcAirport1Valid.json"));
            HttpContent multipleTransferRouteSrcAirport2ValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteSrcAirport2Valid.json"));
            HttpContent multipleTransferRouteDestAirportValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteDestAirportValid.json"));
            HttpContent multipleTransferRouteSrcAirport1Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteSrcAirport1.json"));
            HttpContent multipleTransferRouteSrcAirport2Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteSrcAirport2.json"));
            HttpContent multipleTransferRouteAirline1Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteAirline1.json"));
            HttpContent multipleTransferRouteAirline2Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteAirline2.json"));

            HttpContent multipleTransferRouteMaxTransferCountSrcAirport1ValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport1Valid.json"));
            HttpContent multipleTransferRouteMaxTransferCountSrcAirport2ValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport2Valid.json"));
            HttpContent multipleTransferRouteMaxTransferCountSrcAirport3ValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport3Valid.json"));
            HttpContent multipleTransferRouteMaxTransferCountSrcAirport4ValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport4Valid.json"));
            HttpContent multipleTransferRouteMaxTransferCountDestAirportValidCntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountDestAirportValid.json"));
            HttpContent multipleTransferRouteMaxTransferCountSrcAirport1Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport1.json"));
            HttpContent multipleTransferRouteMaxTransferCountSrcAirport2Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport2.json"));
            HttpContent multipleTransferRouteMaxTransferCountSrcAirport3Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport3.json"));
            HttpContent multipleTransferRouteMaxTransferCountSrcAirport4Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountSrcAirport4.json"));
            HttpContent multipleTransferRouteMaxTransferCountAirline1Cntt = new StringContent(LoadJson("Jsons/multipleTransferRouteMaxTransferCountAirline1.json"));

            HttpContent notExistAirportCnt = new StringContent(LoadJson("Jsons/notExistAirport.json"));

            Mock<IHttpSender> mockContainer = new Mock<IHttpSender>();
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.ExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = existRouteSrcAirportValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.ExistRouteDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = existRouteDestAirportValidCntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.ExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = existRouteSrcAirportCntt }));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.ExistRouteAirline, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = existRouteAirlineCntt }));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.NotExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = notExistRouteSrcAirportValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.NotExistRouteDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = notExistRouteDestAirportValidCntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.NotExistRouteSrcAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = notExistRouteSrcAirportCntt }));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteSrcAirport1ValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteSrcAirport2ValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteDestAirportValidCntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteSrcAirport1Cntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteSrcAirport2Cntt }));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.MultipleTransferRouteAirline1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteAirline1Cntt }));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.MultipleTransferRouteAirline2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteAirline2Cntt }));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport1ValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport2ValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport3, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport3ValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport4, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport4ValidCntt }));
            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.MultipleTransferRouteMaxTransferCountDestAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountDestAirportValidCntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport1Cntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport1Cntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport3, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport1Cntt }));
            mockContainer.Setup(x => x.GetDataForRoute(TestConsts.MultipleTransferRouteMaxTransferCountSrcAirport4, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountSrcAirport1Cntt }));
            mockContainer.Setup(x => x.GetDataForAirline(TestConsts.MultipleTransferRouteMaxTransferCountAirline1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = multipleTransferRouteMaxTransferCountAirline1Cntt }));

            mockContainer.Setup(x => x.GetDataForAirport(TestConsts.NotExistAirport, It.IsAny<CancellationToken>())).Returns(Task.FromResult(new HttpResponseMessage() { Content = notExistAirportCnt }));
            return mockContainer.Object;
        }

        [SetUp]
        public void Setup()
        {
            IHttpSender sender = MockIHttpSender();
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
