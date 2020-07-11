using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Mapper.Functions.Tests.Unit
{
    public class HttpRequestProcessorTests
    {
        const string _body = "hello world";
        const HttpStatusCode _statusCode = HttpStatusCode.OK;
        const int _delay = 0;

        private readonly HttpRequestProcessor _sut;
        private readonly Mock<IHttpMockEngine> _mockHttpMockEngine;
        private readonly Mock<IHttpHistoryService> _mockHttpMockHistoryService;
        private readonly Mock<IMapper<HttpRequestObject, Task<HttpRequestDetails>>> _mockMapper;

        public HttpRequestProcessorTests()
        {
            _mockHttpMockEngine = new Mock<IHttpMockEngine>();
            _mockHttpMockHistoryService = new Mock<IHttpHistoryService>();
            _mockMapper = new Mock<IMapper<HttpRequestObject, Task<HttpRequestDetails>>>();
            _sut = new HttpRequestProcessor(_mockHttpMockEngine.Object, _mockHttpMockHistoryService.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task HttpRequestProcessorReturnsHttpResponseMessageMultipleHeaders()
        {
            var headers = new Dictionary<string, List<string>>()
            {
                { "responseHeader1", new List<string> { "value" } },
                { "auth", new List<string> { "value2" } },
            };

            SetUpHttpMockEngineMock(headers);
            var actual = await _sut.ProcessAsync(BuildHttpRequestObject());

            var actualBody = await actual.Content.ReadAsStringAsync();
            var actualHeaders = actual.Content.Headers.ToDictionary(a => a.Key, a => a.Value);

            Assert.Equal(_statusCode, actual.StatusCode);
            Assert.Equal(_body, actualBody);

            foreach (var header in headers)
            {
                Assert.True(actualHeaders.TryGetValue(header.Key, out var value));
                Assert.Equal(header.Value, value);
            }
        }
        
        [Fact]
        public async Task HttpRequestProcessorLogsDataToHttpMockHistoryService()
        {
            const string route = "route1";
            const string body = "body1";

            SetUpHttpMockEngineMock(new Dictionary<string, List<string>>());

            var method = HttpMethod.Get;
            _mockMapper.Setup(m => m.Map(It.IsAny<HttpRequestObject>()))
                .Returns(Task.FromResult(new HttpRequestDetails(method, route, body,
                new Dictionary<string, List<string>>(), new Dictionary<string, string>())));

            await _sut.ProcessAsync(BuildHttpRequestObject());

            _mockHttpMockHistoryService.Verify(m => m.AddAsync(It.Is<HttpRequestDetails>(
                r => r.Body == body
                && r.Route == route
                && r.Method == method)));
        }

        private static HttpRequestObject BuildHttpRequestObject() =>
                    new HttpRequestObject(new MemoryStream(), HttpMethod.Get,
                        new Dictionary<string, string>(), null);

        private void SetUpHttpMockEngineMock(Dictionary<string, List<string>> headers) => 
            _mockHttpMockEngine.Setup(m => m.Process(It.IsAny<HttpRequestDetails>()))
                .Returns(new HttpAction(_statusCode, _body, headers, _delay));
    }
}
