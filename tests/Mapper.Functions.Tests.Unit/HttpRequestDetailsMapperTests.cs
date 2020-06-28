using Mocker.Application;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.IO;
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
        public async Task MapsToHttpRequestDetailsAsync()
        {
            const string body = "body data";
            var query = new Dictionary<string, string>()
                {
                    { "name", "mark" }
                };

            var httpRequestObject = new HttpRequestObject(new MemoryStream(Encoding.Default.GetBytes(body)),
                HttpMethod.Get, query, null);

            HttpRequestDetails actual = await _sut.Map(httpRequestObject);

            Assert.Equal(body, actual.Body);
            Assert.Equal(HttpMethod.Get, actual.Method);
            Assert.Equal(query, actual.Query);
            Assert.Null(actual.Route);
        }
    }
}
