using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
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
        private readonly Mock<IMapper<HttpRequestObject, Task<HttpRequestDetails>>> _mockMapper;

        public HttpRequestProcessorTests()
        {
            _mockHttpMockEngine = new Mock<IHttpMockEngine>();
            _mockMapper = new Mock<IMapper<HttpRequestObject, Task<HttpRequestDetails>>>();
            _sut = new HttpRequestProcessor(_mockHttpMockEngine.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task HttpRequestProcessorReturnsHttpResponseMessageAsyncMultipleHeaders()
        {
            var headers = new Dictionary<string, List<string>>()
            {
                { "responseHeader1", new List<string> { "value" } },
                { "auth", new List<string> { "value2" } },
            };

            SetUpHttpMockEngineMock(headers);

            var httpRequestObject = new HttpRequestObject(new MemoryStream(), HttpMethod.Get,
                new Dictionary<string, string>(), null);
            var actual = await _sut.ProcessAsync(httpRequestObject);

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

        private void SetUpHttpMockEngineMock(Dictionary<string, List<string>> headers) => 
            _mockHttpMockEngine.Setup(m => m.Process(It.IsAny<HttpRequestDetails>()))
                .Returns(new HttpAction(_statusCode, _body, headers, _delay));
    }
}
