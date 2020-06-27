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
        private Mock<IMockRuleRepository> _mockMockRuleRepository;

        public HttpMockEngineTests()
        {
            _mockMockRuleRepository = new Mock<IMockRuleRepository>();
            _sut = new HttpMockEngine(_mockMockRuleRepository.Object);
        }

        [Fact]
        public void WhenNoRequestFilterReturnsDefaultResponse()
        {
            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, string.Empty, string.Empty, 
                new Dictionary<string, string>(), string.Empty);

            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(string.Empty, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public void ReturnsCorrectResponseForGetWithBody()
        {
            var httpMockResponse = new HttpMockResponse(HttpStatusCode.NotFound, "Can't find it!");
            const string inputBody = "Hello world!";
            _mockMockRuleRepository.Setup(m => m.GetAllMocks()).Returns(new List<MockRule>()
            {
                new HttpMockRule(
                    new HttpRequestFilter(HttpMethod.Get, inputBody),
                    httpMockResponse
                )
            });

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, null, inputBody,
                new Dictionary<string, string>(), string.Empty);
            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(httpMockResponse.Body, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(httpMockResponse.StatusCode, actual.StatusCode);
        }

        [Fact]
        public void ReturnsCorrectResponseForGetWithNoBody()
        {
            var httpMockResponse = new HttpMockResponse(HttpStatusCode.NotFound, "Can't find it!");
            _mockMockRuleRepository.Setup(m => m.GetAllMocks()).Returns(new List<MockRule>()
            {
                new HttpMockRule(
                    new HttpRequestFilter(HttpMethod.Get, string.Empty),
                    httpMockResponse
                )
            });

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, null, string.Empty,
                new Dictionary<string, string>(), string.Empty);
            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(httpMockResponse.Body, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(httpMockResponse.StatusCode, actual.StatusCode);
        }

        [Fact]
        public void ReturnsCorrectResponseForGetWithBodyAndRoute()
        {
            var httpMockResponse = new HttpMockResponse(HttpStatusCode.NotFound, "Can't find it!");
            const string inputBody = "Hello world";
            const string route = "getStuff";
            _mockMockRuleRepository.Setup(m => m.GetAllMocks()).Returns(new List<MockRule>()
            {
                new HttpMockRule(
                    new HttpRequestFilter(HttpMethod.Get, inputBody){ Route = route },
                    httpMockResponse
                )
            });

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, route, inputBody,
                new Dictionary<string, string>(), string.Empty);
            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(httpMockResponse.Body, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, string>(), actual.Headers);
            Assert.Equal(httpMockResponse.StatusCode, actual.StatusCode);
        }
    }
}
