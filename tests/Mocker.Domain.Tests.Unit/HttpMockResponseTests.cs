using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpMockResponseTests
    {
        [Fact]
        public void CreatesValidHttpMockResponse()
        {
            var statusCode = HttpStatusCode.OK;
            var body = "{\"name\": \"Mark\"}";
            var actual = new HttpAction(statusCode, body);

            Assert.Equal(statusCode, actual.StatusCode);
            Assert.Equal(body, actual.Body);
        }

        [Fact]
        public void CreatesValidHttpMockResponseWithHeaders()
        {
            var statusCode = HttpStatusCode.OK;
            var body = "{\"name\": \"Mark\"}";
            var headers = new Dictionary<string, List<string>>()
            {
                { "header1", new List<string> { "value" } }
            };

            var actual = new HttpAction(statusCode, body, headers);

            Assert.Equal(statusCode, actual.StatusCode);
            Assert.Equal(body, actual.Body);
            Assert.Equal(headers, actual.Headers);
        }

        [Fact]
        public void CreatesValidHttpMockResponseWithHeadersAndDelay()
        {
            var statusCode = HttpStatusCode.OK;
            var body = "{\"name\": \"Mark\"}";
            var delay = 500;
            var headers = new Dictionary<string, List<string>>()
            {
                { "header1", new List<string> { "value" } }
            };

            var actual = new HttpAction(statusCode, body, headers, delay);

            Assert.Equal(statusCode, actual.StatusCode);
            Assert.Equal(body, actual.Body);
            Assert.Equal(headers, actual.Headers);
            Assert.Equal(delay, actual.Delay);
        }
    }
}
