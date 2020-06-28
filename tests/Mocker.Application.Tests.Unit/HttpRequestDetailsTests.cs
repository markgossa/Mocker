using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpRequestDetailsTests
    {
        [Fact]
        public void CreateValidHttpRequestDetails()
        {
            var httpMethod = HttpMethod.Post;
            var route = "route1";
            var body = "Hello world!";
            var headers = new Dictionary<string, IEnumerable<string>>();
            var queryString = new Dictionary<string, string>()
            {
                { "name", "mark" }
            };

            var actual = new HttpRequestDetails(httpMethod, route, body,
                headers, queryString);

            Assert.Equal(httpMethod, actual.Method);
            Assert.Equal(route, actual.Route);
            Assert.Equal(body, actual.Body);
            Assert.Equal(headers, actual.Headers);
            Assert.Equal(queryString, actual.Query);
        }
    }
}
