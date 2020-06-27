using System.Net;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpMockTests
    {
        [Fact]
        public void CreatesValidEmptyHttpMock()
        {
            var actual = new HttpMockRule();

            Assert.IsType<HttpMockRule>(actual);
        }

        [Fact]
        public void CreatesValidHttpMock()
        {
            var httpMockResponse = new HttpMockAction(HttpStatusCode.OK, "hello world");
            var actual = new HttpMockRule(httpMockResponse);

            Assert.Equal(httpMockResponse, actual.HttpMockResponse);
        }
    }
}
