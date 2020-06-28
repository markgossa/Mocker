using Mocker.Functions.Models;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Mapper.Functions.Tests.Unit
{
    public class HttpRequestObjectTests
    {
        [Fact]
        public void CreatesValidHttpRequestObject()
        {
            var bodyStream = new MemoryStream(Encoding.Default.GetBytes("hello world"));
            var method = HttpMethod.Get;
            var route = "route1";
            var query = new Dictionary<string, string>()
            {
                { "name", "mark" }
            };

            var actual = new HttpRequestObject(bodyStream, method, query, route);

            Assert.Equal(bodyStream, actual.BodyStream);
            Assert.Equal(method, actual.Method);
            Assert.Equal(route, actual.Route);
            Assert.Equal(query, actual.Query);
        }
    }
}
