using Mocker.Application.Contracts;
using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpRuleEngineTests
    {
        private const string _route = "route1";
        private const string _body = "hello world";
        private const string _bodyNoHeaders = "I don't have headers";
        private const string _queryKey = "name";
        private const string _queryValue = "mark";
        private readonly Dictionary<string, string> _query = new Dictionary<string, string>()
        {
            { _queryKey, _queryValue}
        };

        private readonly HttpRuleEngine _sut;
        private readonly Mock<IHttpRuleRepository> _mockHttpRuleRepository;

        public HttpRuleEngineTests()
        {
            _mockHttpRuleRepository = new Mock<IHttpRuleRepository>();
            _sut = new HttpRuleEngine(_mockHttpRuleRepository.Object);
        }

        [Fact]
        public async Task WhenNoRequestFilterReturnsDefaultResponse()
        {
            var actual = await _sut.Process(BuildHttpRequestDetails());

            Assert.Equal(string.Empty, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, List<string>>(), actual.Headers);
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task MakesCorrectCallToRepository()
        {
            await _sut.Process(BuildHttpRequestDetails());

            _mockHttpRuleRepository.Verify(m => m.FindAsync(It.Is<HttpMethod>(h => h == HttpMethod.Get),
                It.Is<string>(s => s == _body),
                It.Is<string>(r => r == _route)));
        }

        [Fact]
        public async Task ReturnsFirstMatchingRuleFromRepositoryIfMultipleMatchesFound()
        {
            _mockHttpRuleRepository.Setup(m => m.FindAsync(It.IsAny<HttpMethod>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query),
                        new HttpAction(HttpStatusCode.OK, _body)
                    ),
                    new HttpRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query),
                        new HttpAction(HttpStatusCode.OK, "body")
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body,
                new Dictionary<string, List<string>>(), _query);

            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsMatchingRuleIfRequestMatchesRuleWithSingleHeader()
        {
            var ruleHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } }
            };

            SetUpMockRuleRepositoryWithMockRulesWithHeaders(ruleHeaders);

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body, AddHeadersToRequest(ruleHeaders), _query);
            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsMatchingRuleIfRequestMatchesRuleWithMultipleHeaders()
        {
            var ruleHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } },
                { "name", new List<string>(){ "mark" } }
            };

            SetUpMockRuleRepositoryWithMockRulesWithHeaders(ruleHeaders);

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body, AddHeadersToRequest(ruleHeaders), _query);
            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsMatchingRuleWithoutHeadersIfRuleHasHeadersAndIgnoreHeadersTrue()
        {
            var ruleHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } },
                { "name", new List<string>(){ "mark" } }
            };

            _mockHttpRuleRepository.Setup(m => m.FindAsync(It.IsAny<HttpMethod>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query, ruleHeaders, true),
                        new HttpAction(HttpStatusCode.OK, _body)
                    )
                }));

            var requestHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } },
                { "name", new List<string>(){ "mark" } }
            };

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body, requestHeaders, _query);
            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsDefaultResponseIfRequestHeadersDoNotMatchRuleHeadersAndIgnoreHeadersFalse()
        {
            var ruleHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } },
                { "name", new List<string>(){ "mark" } }
            };

            _mockHttpRuleRepository.Setup(m => m.FindAsync(It.IsAny<HttpMethod>(),
                It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query, ruleHeaders, false),
                        new HttpAction(HttpStatusCode.OK, _body)
                    )
                }));

            var requestHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } }
            };

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body, requestHeaders, _query);
            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(string.Empty, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, List<string>>(), actual.Headers);
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        private void SetUpMockRuleRepositoryWithMockRulesWithHeaders(Dictionary<string, List<string>> ruleHeaders) => 
            _mockHttpRuleRepository.Setup(m => m.FindAsync(It.IsAny<HttpMethod>(), 
                It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query, ruleHeaders),
                        new HttpAction(HttpStatusCode.OK, _body)
                    ),
                    new HttpRule(
                        new HttpFilter(HttpMethod.Get, _body, _route, _query),
                        new HttpAction(HttpStatusCode.OK, _bodyNoHeaders)
                    )
                }));

        private static Dictionary<string, List<string>> AddHeadersToRequest(Dictionary<string, List<string>> ruleHeaders) => 
            new Dictionary<string, List<string>>(ruleHeaders)
            {
                { "additionalHeader", new List<string> { "value" } }
            };

        private HttpRequestDetails BuildHttpRequestDetails() => new HttpRequestDetails(HttpMethod.Get, _route, _body,
                new Dictionary<string, List<string>>(), _query);
    }
}
