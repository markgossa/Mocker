using Mocker.Functions.Models;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Mocker.Functions.Tests.Unit
{
    public class HttpRuleActionRequestTests
    {
        [Fact]
        public void CreateHttpRuleActionRequestWithAllProperties()
        {
            var expectedBody = "Hello world";
            var expectedDelay = 500;
            var expectedStatusCode = HttpStatusCode.BadRequest;
            var expectedHeaders = new Dictionary<string, List<string>>
            {
                { "header1", new List<string>{ "value1", "value2" } },
                { "header2", new List<string>{ "value1", "value2" } }
            };

            var actual = new HttpRuleActionRequest
            {
                Body = expectedBody,
                Delay = expectedDelay,
                Headers = expectedHeaders,
                StatusCode = expectedStatusCode
            };

            Assert.Equal(expectedBody, actual.Body);
            Assert.Equal(expectedDelay, actual.Delay);
            Assert.Equal(expectedHeaders, actual.Headers);
            Assert.Equal(expectedStatusCode, actual.StatusCode);
        }
    }
}
