using Mocker.Functions.Models;
using System.Collections.Generic;
using Xunit;

namespace Mocker.Functions.Tests.Unit
{
    public class HttpRuleFilterRequestTests
    {
        [Fact]
        public void CreatesHttpRuleRequestFilterWithAllProperties()
        {
            var expectedBody = "Hello world!";
            var expectedHeaders = new Dictionary<string, List<string>>
            {
                { "header1", new List<string>{ "value1", "value2" } },
                { "header2", new List<string>{ "value2", "value3" } }
            };
            
            var expectedMethod = "GET";
            var expectedQuery = new Dictionary<string, string>
            {
                { "name", "Mark" }
            };

            var expectedRoute = "route1";

            var actual = new HttpRuleFilterRequest
            {
                Body = expectedBody,
                Headers = expectedHeaders,
                Method = expectedMethod,
                Query = expectedQuery,
                Route = expectedRoute
            };

            Assert.Equal(expectedBody, actual.Body);
            Assert.Equal(expectedHeaders, actual.Headers);
            Assert.Equal(expectedMethod, actual.Method);
            Assert.Equal(expectedQuery, actual.Query);
            Assert.Equal(expectedRoute, actual.Route);
        }
    }
}
