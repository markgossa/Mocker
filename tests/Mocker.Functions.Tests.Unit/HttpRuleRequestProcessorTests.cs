using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Mocker.Functions.Tests.Unit
{
    public class HttpRuleRequestProcessorTests
    {
        private readonly Mock<IHttpRuleRepository> _mockHttpRuleRepository = new Mock<IHttpRuleRepository>();
        private readonly HttpRuleRequestProcessor _sut;

        public HttpRuleRequestProcessorTests()
        {
            _sut = new HttpRuleRequestProcessor(_mockHttpRuleRepository.Object);
        }

        [Fact]
        public async Task AddAsyncSubmitsDataToHttpRuleRepository()
        {
            var expectedFilterBody = "Hello world!";
            var expectedFilterHeaders = new Dictionary<string, List<string>>
            {
                { "header1", new List<string> { "value3" } }
            };
            var expectedFilterMethod = "GET";
            var expectedFilterQuery = new Dictionary<string, string>
            {
                { "name", "mark" }
            };
            var expectedFilterRoute = "route1";
            var expectedFilterIgnoreHeaders = false;

            var expectedActionBody = "Hey back!";
            var expectedActionDelay = 500;
            var expectedActionStatusCode = HttpStatusCode.OK;
            var expectedActionHeaders = new Dictionary<string, List<string>>
            {
                { "header1", new List<string> { "value1", "value2" } },
                { "header2", new List<string> { "value1", "value2" } }
            };

            var newRule = new HttpRuleRequest()
            {
                Filter = new HttpRuleFilterRequest()
                {
                    Body = expectedFilterBody,
                    Headers = expectedFilterHeaders,
                    IgnoreHeaders = expectedFilterIgnoreHeaders,
                    Method = expectedFilterMethod,
                    Query = expectedFilterQuery,
                    Route = expectedFilterRoute
                },

                Action = new HttpRuleActionRequest()
                {
                    Body = expectedActionBody,
                    Delay = expectedActionDelay,
                    Headers = expectedActionHeaders,
                    StatusCode = expectedActionStatusCode
                }
            };

            await _sut.AddAsync(newRule);

            _mockHttpRuleRepository.Verify(m => m.AddAsync(It.Is<HttpRule>(
                h => h.HttpFilter.Body == expectedFilterBody
                && h.HttpFilter.Headers == expectedFilterHeaders
                && h.HttpFilter.IgnoreHeaders == expectedFilterIgnoreHeaders
                && h.HttpFilter.Method == new HttpMethod(expectedFilterMethod)
                && h.HttpFilter.Query == expectedFilterQuery
                && h.HttpFilter.Route == expectedFilterRoute
                && h.HttpAction.Body == expectedActionBody
                && h.HttpAction.Delay == expectedActionDelay
                && h.HttpAction.Headers == expectedActionHeaders
                && h.HttpAction.StatusCode == expectedActionStatusCode
                )));
        }
    }
}
