using Mocker.Application.Contracts;
using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Moq;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task ReturnsDefaultResponseWhenNoMatchingRule()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()));

            var actual = await _sut.Process(BuildHttpRequestDetails());

            Assert.Equal(string.Empty, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, List<string>>(), actual.Headers);
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task ReturnsCorrectResponseWhenRuleMatchesAnyRequest()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>
            {
                new HttpRule(
                        new HttpFilter(null, null, null, null),
                        new HttpAction(HttpStatusCode.OK, _body)
                    )
            }));

            var actual = await _sut.Process(BuildHttpRequestDetails());

            Assert.Equal(_body, actual.Body);
            Assert.Equal(0, actual.Delay);
            Assert.Equal(new Dictionary<string, List<string>>(), actual.Headers);
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task MakesCorrectCallToRepository()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()));

            await _sut.Process(BuildHttpRequestDetails());

            _mockHttpRuleRepository.Verify(m => m.GetAllAsync());
        }

        [Fact]
        public async Task ReturnsCorrectHttpActionBasedOnMethodOnly()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(HttpMethod.Post, null, null, null),
                        new HttpAction(HttpStatusCode.OK, "body")
                    ),
                    new HttpRule(
                        new HttpFilter(HttpMethod.Get, null, null, null),
                        new HttpAction(HttpStatusCode.OK, _body)
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Post, _route, _body,
                new Dictionary<string, List<string>>(), _query);

            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal("body", actual.Body);
        }

        [Fact]
        public async Task ReturnsCorrectHttpActionBasedOnBodyOnly()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(null, "bodyFilter", null, null),
                        new HttpAction(HttpStatusCode.OK, _body)
                    ),
                    new HttpRule(
                        new HttpFilter(HttpMethod.Post, null, null, null),
                        new HttpAction(HttpStatusCode.OK, "body")
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Post, _route, "bodyFilter",
                new Dictionary<string, List<string>>(), _query);

            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsCorrectHttpActionBasedOnRouteOnly()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {
                    
                    new HttpRule(
                        new HttpFilter(HttpMethod.Post, null, null, null),
                        new HttpAction(HttpStatusCode.OK, "body")
                    ),
                    new HttpRule(
                        new HttpFilter(null, null, "route11", null),
                        new HttpAction(HttpStatusCode.OK, _body)
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Delete, "route11", null,
                new Dictionary<string, List<string>>(), null);

            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsCorrectHttpActionBasedOnMethodAndBody()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {

                    new HttpRule(
                        new HttpFilter(HttpMethod.Delete, "body", null, null),
                        new HttpAction(HttpStatusCode.OK, _body)
                    ),
                    new HttpRule(
                        new HttpFilter(HttpMethod.Delete, null, null, null),
                        new HttpAction(HttpStatusCode.OK, "body")
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Delete, "route11", "body",
                new Dictionary<string, List<string>>(), null);

            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }
        
        [Fact]
        public async Task ReturnsCorrectHttpActionBasedOnMethodBodyAndRoute()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(HttpMethod.Delete, "body", "route11", null),
                        new HttpAction(HttpStatusCode.OK, _body)
                    ),
                    new HttpRule(
                        new HttpFilter(HttpMethod.Delete, null, null, null),
                        new HttpAction(HttpStatusCode.OK, "body")
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Delete, "route11", "body",
                new Dictionary<string, List<string>>(), null);

            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsCorrectHttpActionBasedOnQueryOnly()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(null, null, null, new Dictionary<string, string>(_query){ { "additional", "value" } }),
                        new HttpAction(HttpStatusCode.OK, "incorrect body")
                    ),
                    new HttpRule(
                        new HttpFilter(null, null, null, _query),
                        new HttpAction(HttpStatusCode.OK, _body)
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Delete, "route11", null,
                new Dictionary<string, List<string>>(), _query);

            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsFirstMatchingRuleFromRepositoryIfMultipleMatchesFound()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
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
        public async Task ReturnsMatchingRuleIfRequestMatchesRuleWithOnlyHeader()
        {
            var ruleHeaders = new Dictionary<string, List<string>>()
            {
                { "authValue", new List<string>(){ "password" } }
            };

            var filterHeaders = new Dictionary<string, List<string>>()
            {
                { "newHeader1", new List<string>{ "value1" } },
                { "newHeader2", new List<string>{ "value1" } }
            };

            var requestHeaders = new Dictionary<string, List<string>>()
            {
                { "newHeader2", new List<string>{ "value1" } },
                { "newHeader1", new List<string>{ "value1" } },
                { "cache-control", new List<string>{ "nocache" } },
                { "uniqueId", new List<string>{ "id" } }
            };

            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(null, null, null, null, ruleHeaders),
                        new HttpAction(HttpStatusCode.OK, "incorrect body"),
                        1
                    ),
                    new HttpRule(
                        new HttpFilter(null, null, null, null, filterHeaders),
                        new HttpAction(HttpStatusCode.OK, _body),
                        2
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body, requestHeaders, _query);
            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsMatchingRuleIfRequestMatchesRuleWithOnlyHeader2()
        {
            var ruleHeaders = new Dictionary<string, List<string>>()
            {
                { "newHeader1", new List<string>{ "value1" } },
                { "newHeader2", new List<string>{ "value2" } }
            };

            var filterHeaders = new Dictionary<string, List<string>>()
            {
                { "newHeader1", new List<string>{ "value1" } },
                { "newHeader2", new List<string>{ "value1" } }
            };

            var requestHeaders = new Dictionary<string, List<string>>()
            {
                { "newHeader2", new List<string>{ "value1" } },
                { "newHeader1", new List<string>{ "value1" } },
                { "cache-control", new List<string>{ "nocache" } },
                { "uniqueId", new List<string>{ "id" } }
            };

            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>()
                {
                    new HttpRule(
                        new HttpFilter(null, null, null, null, ruleHeaders),
                        new HttpAction(HttpStatusCode.OK, "incorrect body"),
                        1
                    ),
                    new HttpRule(
                        new HttpFilter(null, null, null, null, filterHeaders),
                        new HttpAction(HttpStatusCode.OK, _body),
                        2
                    )
                }));

            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body, requestHeaders, _query);
            var actual = await _sut.Process(httpRequestDetails);

            Assert.Equal(_body, actual.Body);
        }

        [Fact]
        public async Task ReturnsCorrectResponseWithDelayIfSpecified()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(new List<HttpRule>
            {
                new HttpRule(
                        new HttpFilter(null, null, null, null),
                        new HttpAction(HttpStatusCode.OK, _body, null, 500)
                    )
            }));

            var stopwatch = Stopwatch.StartNew();
            var actual = await _sut.Process(BuildHttpRequestDetails());
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 480, 520);
        }

        private HttpRequestDetails BuildHttpRequestDetails() => new HttpRequestDetails(HttpMethod.Get, _route, _body,
                new Dictionary<string, List<string>>(), _query);
    }
}
