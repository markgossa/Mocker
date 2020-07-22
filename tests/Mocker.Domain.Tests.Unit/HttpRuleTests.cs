using Mocker.Domain.Models.Http;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpRuleTests
    {
        [Fact]
        public void CreatesValidHttpRule()
        {
            var httpAction = new HttpAction(HttpStatusCode.OK, "hello world");
            var httpFilter = new HttpFilter(HttpMethod.Delete, "body");
            var actual = new HttpRule(httpFilter, httpAction, 1);

            Assert.Equal(httpAction, actual.HttpAction);
            Assert.Equal(httpFilter, actual.HttpFilter);
            Assert.Equal(1, actual.Id);
        }
    }
}
