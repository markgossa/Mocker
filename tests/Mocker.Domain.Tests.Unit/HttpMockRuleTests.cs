using Mocker.Domain.Models.Http;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpMockRuleTests
    {
        [Fact]
        public void CreatesValidMockRule()
        {
            var httpAction = new HttpAction(HttpStatusCode.OK, "hello world");
            var httpFilter = new HttpFilter(HttpMethod.Delete, "body");
            var actual = new HttpRule(httpFilter, httpAction);

            Assert.Equal(httpAction, actual.HttpAction);
        }
    }
}
