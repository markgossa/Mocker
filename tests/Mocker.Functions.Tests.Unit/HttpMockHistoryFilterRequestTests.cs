using Mocker.Functions.Models;
using System.Net.Http;
using Xunit;

namespace Mapper.Functions.Tests.Unit
{
    public class HttpMockHistoryFilterRequestTests
    {
        [Fact]
        public void CreatesValidHttpMockHistoryFilterRequest()
        {
            var method = HttpMethod.Delete;
            var route = "route1";
            var body = "body1";
            var actual = new HttpMockHistoryFilterRequest(method, route, body);

            Assert.Equal(method, actual.Method);
            Assert.Equal(route, actual.Route);
            Assert.Equal(body, actual.Body);
        }
    }
}
