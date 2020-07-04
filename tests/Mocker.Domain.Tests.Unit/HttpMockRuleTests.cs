using Mocker.Domain.Models.Http;
using System.Net;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpMockRuleTests
    {
        [Fact]
        public void CreatesValidMockRule()
        {
            var httpMockResponse = new HttpAction(HttpStatusCode.OK, "hello world");
            var actual = new HttpRule(httpMockResponse);

            Assert.Equal(httpMockResponse, actual.HttpAction);
        }
    }
}
