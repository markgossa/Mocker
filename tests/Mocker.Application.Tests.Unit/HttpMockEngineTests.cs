using Mocker.Domain;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpMockEngineTests
    {
        private const string _route = "route1";
        private const string _body = "hello world";
        private const string _queryKey = "name";
        private const string _queryValue = "mark";

        private readonly Dictionary<string, string> _query = new Dictionary<string, string>()
        {
            { _queryKey, _queryValue}
        };

        private readonly HttpMockEngine _sut;
        private readonly Mock<IMockHttpRuleRepository> _mockMockRuleRepository;

        public HttpMockEngineTests()
        {
            _mockMockRuleRepository = new Mock<IMockHttpRuleRepository>();
            _sut = new HttpMockEngine(_mockMockRuleRepository.Object);
        }

        [Fact]
        public void WhenNoRequestFilterReturnsDefaultResponse()
        {
            var actual = _sut.Process(BuildHttpRequestDetails());

            Assert.Equal(string.Empty, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, List<string>>(), actual.Headers);
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public void MakesCorrectCallToRepository()
        {
            _sut.Process(BuildHttpRequestDetails());

            _mockMockRuleRepository.Verify(m => m.Find(It.Is<HttpMethod>(h => h == HttpMethod.Get),
                It.Is<Dictionary<string, string>>(q => q.Count == 1 && q.ContainsKey(_queryKey) && q.ContainsValue(_queryValue)),
                It.Is<string>(s => s == _body),
                It.Is<string>(r => r == _route)));
        }

        [Fact]
        public void ReturnsFirstMatchingRuleFromRepositoryIfMultipleMatchesFound()
        {
            _mockMockRuleRepository.Setup(m => m.Find(It.IsAny<HttpMethod>(), It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(new List<HttpMockRule>()
                {
                    new HttpMockRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query),
                        new HttpMockAction(HttpStatusCode.OK, _body)
                    ),
                    new HttpMockRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query),
                        new HttpMockAction(HttpStatusCode.OK, "body")
                    )
                });

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body,
                new Dictionary<string, List<string>>(), _query);

            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public void ReturnsMatchingRuleIfRequestContainsRuleSingleHeader()
        {
            var filterHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } }
            };

            _mockMockRuleRepository.Setup(m => m.Find(It.IsAny<HttpMethod>(), It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(new List<HttpMockRule>()
                {
                    new HttpMockRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query),
                        new HttpMockAction(HttpStatusCode.OK, "I don't have headers")
                    ),
                    new HttpMockRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query, filterHeaders),
                        new HttpMockAction(HttpStatusCode.OK, _body)
                    )
                });

            var receivedHeaders = new Dictionary<string, List<string>>(filterHeaders)
            {
                { "additionalHeader", new List<string> { "value" } }
            };

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body,
                receivedHeaders, _query);

            var actual = _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        private HttpRequestDetails BuildHttpRequestDetails() => new HttpRequestDetails(HttpMethod.Get, _route, _body,
                new Dictionary<string, List<string>>(), _query);
    }
}
