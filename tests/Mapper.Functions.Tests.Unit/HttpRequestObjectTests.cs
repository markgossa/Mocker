using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
        public void CreatesValidHttpRequestObjectWithHeaders()
        {
            var bodyStream = new MemoryStream(Encoding.Default.GetBytes("hello world"));
            var method = HttpMethod.Get;
            var route = "route1";
            var query = new Dictionary<string, string>()
            {
                { "name", "mark" }
            };

            var headers = new Dictionary<string, StringValues>()
            {
                { "name", new StringValues("mark") }
            };

            var actual = new HttpRequestObject(bodyStream, method, query, new HeaderDictionary(headers), route);

            Assert.Equal(bodyStream, actual.BodyStream);
            Assert.Equal(method, actual.Method);
            Assert.Equal(route, actual.Route);
            Assert.Equal(query, actual.Query);
            Assert.Equal(headers, actual.Headers);
        }
    }
}
