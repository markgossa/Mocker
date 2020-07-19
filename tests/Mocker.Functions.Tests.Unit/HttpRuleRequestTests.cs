using Mocker.Functions.Models;
using Xunit;

namespace Mocker.Functions.Tests.Unit
{
    public class HttpRuleRequestTests
    {
        [Fact]
        public void CreatesNewHttpRuleRequestWithAllProperties()
        {
            var httpRuleFilterRequest = new HttpRuleFilterRequest();
            var httpRuleActionRequest = new HttpRuleActionRequest();
            var request = new HttpRuleRequest()
            {
                Filter = httpRuleFilterRequest,
                Action = httpRuleActionRequest
            };

            Assert.Equal(httpRuleFilterRequest, request.Filter);
            Assert.Equal(httpRuleActionRequest, request.Action);
        }
    }
}
