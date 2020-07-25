using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpActionTests
    {
        [Fact]
        public void CreatesValidHttpAction()
        {
            var statusCode = HttpStatusCode.OK;
            var body = "{\"name\": \"Mark\"}";
            var actual = new HttpAction(statusCode, body);

            Assert.Equal(statusCode, actual.StatusCode);
            Assert.Equal(body, actual.Body);
        }

        [Fact]
        public void CreatesValidHttpActionWithDefaultStatusCode()
        {
            var statusCode = default(HttpStatusCode);
            var body = "{\"name\": \"Mark\"}";
            var actual = new HttpAction(statusCode, body);

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            Assert.Equal(body, actual.Body);
        }

        [Fact]
        public void CreatesValidHttpActionWithHeaders()
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
        public void CreatesValidHttpActionWithHeadersAndDelay()
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
