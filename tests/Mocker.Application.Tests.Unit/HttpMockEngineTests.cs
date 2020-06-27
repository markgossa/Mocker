using Mocker.Domain;
using Mocker.Infrastructure;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpMockEngineTests
    {
        private readonly HttpMockEngine _sut;
        private Mock<IMockHttpRuleRepository> _mockMockRuleRepository;

        public HttpMockEngineTests()
        {
            _mockMockRuleRepository = new Mock<IMockHttpRuleRepository>();
            _sut = new HttpMockEngine(_mockMockRuleRepository.Object);
        }

        [Fact]
        public void WhenNoRequestFilterReturnsDefaultResponse()
        {
            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, string.Empty, string.Empty, 
                new Dictionary<string, string>(), null);

            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(string.Empty, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public void ReturnsCorrectResponseBasedOnMethodAndBody()
        {
            var httpMockResponse = new HttpMockAction(HttpStatusCode.NotFound, "Can't find it!");
            const string inputBody = "Hello world!";
            _mockMockRuleRepository.Setup(m => m.Find(It.IsAny<HttpMethod>(), It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(new List<HttpMockRule>()
            {
                new HttpMockRule(
                    new HttpFilter(HttpMethod.Get, inputBody),
                    httpMockResponse
                )
            });

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, null, inputBody,
                new Dictionary<string, string>(), null);
            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(httpMockResponse.Body, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(httpMockResponse.StatusCode, actual.StatusCode);
        }

        [Fact]
        public void ReturnsCorrectResponseBasedOnMethod()
        {
            var httpMockResponse = new HttpMockAction(HttpStatusCode.NotFound, "Can't find it!");
            _mockMockRuleRepository.Setup(m => m.Find(It.IsAny<HttpMethod>(), It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(new List<HttpMockRule>()
            {
                new HttpMockRule(
                    new HttpFilter(HttpMethod.Get, string.Empty),
                    httpMockResponse
                )
            });

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, null, string.Empty,
                new Dictionary<string, string>(), null);
            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(httpMockResponse.Body, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(httpMockResponse.StatusCode, actual.StatusCode);
        }

        [Fact]
        public void ReturnsCorrectResponseBasedOnMethodBodyAndRoute()
        {
            var httpMockResponse = new HttpMockAction(HttpStatusCode.NotFound, "Can't find it!");
            const string inputBody = "Hello world";
            const string route = "getStuff";
            _mockMockRuleRepository.Setup(m => m.Find(It.IsAny<HttpMethod>(), It.IsAny<Dictionary<string,string>>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(new List<HttpMockRule>()
            {
                new HttpMockRule(
                    new HttpFilter(HttpMethod.Get, inputBody, route, null),
                    httpMockResponse
                )
            });

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, route, inputBody,
                new Dictionary<string, string>(), null);
            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(httpMockResponse.Body, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(httpMockResponse.StatusCode, actual.StatusCode);
        }

        //[Fact]
        //public void ReturnsFirstMatchIfTwoMatches()
        //{
        //    var httpMockResponse = new HttpMockAction(HttpStatusCode.NotFound, "Can't find it!");
        //    const string inputBody = "Hello world";
        //    const string route = "getStuff";
        //    _mockMockRuleRepository.Setup(m => m.GetAll()).Returns(new List<HttpMockRule>()
        //    {
        //        new HttpMockRule(
        //            new HttpFilter(HttpMethod.Get, inputBody, route, null),
        //            httpMockResponse
        //        )
        //    });

        //    var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, route, inputBody,
        //        new Dictionary<string, string>(), string.Empty);
        //    var actual = _sut.Process(httpRequestDetails);

        //    Assert.Equal(httpMockResponse.Body, actual.Body);
        //    Assert.Equal(0, actual.Delay);
        //    Assert.Equal(new Dictionary<string, string>(), actual.Headers);
        //    Assert.Equal(httpMockResponse.StatusCode, actual.StatusCode);
        //}
    }
}
