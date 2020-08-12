using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mapper.Functions.Tests.Unit
{
    public class HttpRequestDetailsMapperTests
    {
        private readonly HttpRequestDetailsMapper _sut;

        public HttpRequestDetailsMapperTests()
        {
            _sut = new HttpRequestDetailsMapper();
        }

        [Fact]
        public async Task MapsValuesToHttpRequestDetails()
        {
            const string body = "body data";
            const string route = "route1";
            var query = new Dictionary<string, string>()
                {
                    { "name", "mark" }
                };

            var httpRequestObject = new HttpRequestObject(new MemoryStream(Encoding.Default.GetBytes(body)),
                HttpMethod.Get, query, new HeaderDictionary(), route);

            var actual = await _sut.Map(httpRequestObject);

            Assert.Equal(body, actual.Body);
            Assert.Equal(HttpMethod.Get, actual.Method);
            Assert.Equal(query, actual.Query);
            Assert.Equal(route, actual.Route);
        }

        [Fact]
        public async Task MapsHeadersToHttpRequestDetails()
        {
            const string body = "body data";
            const string route = "route1";
            var query = new Dictionary<string, string>()
                {
                    { "name", "mark" }
                };

            var headers = new Dictionary<string, StringValues>()
            {
                { "size", new StringValues("10") },
                { "token", new StringValues("password1") }
            };

            var httpRequestObject = new HttpRequestObject(new MemoryStream(Encoding.Default.GetBytes(body)),
                HttpMethod.Get, query, new HeaderDictionary(headers), route);

            var actual = await _sut.Map(httpRequestObject);

            for (var i = 0; i < headers.Count; i++)
            {
                Assert.Equal(headers.ElementAt(i).Key, actual.Headers.ElementAt(i).Key);
                Assert.Equal(headers.ElementAt(i).Value.ToArray(), actual.Headers.ElementAt(i).Value.ToArray());
            }
        }
    }
}
