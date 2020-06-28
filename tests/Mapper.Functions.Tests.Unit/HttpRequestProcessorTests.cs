using Mocker.Application;
using Mocker.Domain;
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
        private readonly List<string> _headerValues = new List<string> { "value" };

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
        public async Task HttpRequestProcessorReturnsHttpResponseMessageAsync()
        {
            SetUpHttpMockEngineMock();

            var httpRequestObject = new HttpRequestObject(new MemoryStream(), HttpMethod.Get,
                new Dictionary<string, string>(), null);
            var actual = await _sut.ProcessAsync(httpRequestObject);

            var actualBody = await actual.Content.ReadAsStringAsync();
            var actualHeaders = actual.Content.Headers.ToDictionary(a => a.Key, a => a.Value);

            Assert.Equal(_statusCode, actual.StatusCode);
            Assert.Equal(_body, actualBody);
            Assert.True(actualHeaders.TryGetValue("responseHeader1", out var value));
            Assert.Equal(_headerValues, value);
        }

        private void SetUpHttpMockEngineMock()
        {
            var headers = new Dictionary<string, List<string>>()
            {
                { "responseHeader1", _headerValues }
            };

            _mockHttpMockEngine.Setup(m => m.Process(It.IsAny<HttpRequestDetails>()))
                .Returns(new HttpMockAction(_statusCode, _body, headers, _delay));
        }
    }
}
