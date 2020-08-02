using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Mocker.Functions.Tests.Unit
{
    public class HttpRuleRequestProcessorTests
    {
        private readonly Mock<IHttpRuleService> _mockHttpRuleRepository = new Mock<IHttpRuleService>();
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
                && h.HttpFilter.Method == new HttpMethod(expectedFilterMethod)
                && h.HttpFilter.Query == expectedFilterQuery
                && h.HttpFilter.Route == expectedFilterRoute
                && h.HttpAction.Body == expectedActionBody
                && h.HttpAction.Delay == expectedActionDelay
                && h.HttpAction.Headers == expectedActionHeaders
                && h.HttpAction.StatusCode == expectedActionStatusCode
                )));
        }
        
        [Fact]
        public async Task AddAsyncSubmitsDataToHttpRuleRepositoryNullMethod()
        {
            var expectedFilterBody = "Hello world!";

            var expectedActionBody = "Hey back!";

            var newRule = new HttpRuleRequest()
            {
                Filter = new HttpRuleFilterRequest()
                {
                    Body = expectedFilterBody,
                },

                Action = new HttpRuleActionRequest()
                {
                    Body = expectedActionBody,
                }
            };

            await _sut.AddAsync(newRule);

            _mockHttpRuleRepository.Verify(m => m.AddAsync(It.Is<HttpRule>(
                h => h.HttpFilter.Body == expectedFilterBody
                && h.HttpFilter.Method == null
                && h.HttpAction.Body == expectedActionBody
                )));
        }

        [Fact]
        public void ValidateReturnsFalseIfInvalidHttpRuleRequest()
        {
            var newRule = new HttpRuleRequest();

            var actual = _sut.Validate(newRule, out _);

            Assert.False(actual);
        }

        [Fact]
        public void ValidateReturnsTrueIfValidHttpRuleRequest()
        {
            var newRule = new HttpRuleRequest()
            {
                Action = new HttpRuleActionRequest
                {
                    StatusCode = HttpStatusCode.OK
                },
                Filter = new HttpRuleFilterRequest()
            };

            var actual = _sut.Validate(newRule, out _);

            Assert.True(actual);
        }

        [Fact]
        public async Task GetsAllHttpRules()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(
                (IEnumerable<HttpRule>) new List<HttpRule> { new HttpRule(new HttpFilter(HttpMethod.Get, string.Empty),
                new HttpAction(HttpStatusCode.OK, string.Empty)) }));

            var actual = await _sut.GetAllAsync();

            Assert.Single(actual.Rules);
            Assert.Equal("GET", actual.Rules.First().Filter.Method);
        }

        [Fact]
        public async Task GetsAllHttpRulesReturnsCorrectFormat()
        {
            _mockHttpRuleRepository.Setup(m => m.GetAllAsync()).Returns(Task.FromResult(
                (IEnumerable<HttpRule>) new List<HttpRule> { new HttpRule(new HttpFilter(HttpMethod.Get, string.Empty),
                new HttpAction(HttpStatusCode.OK, string.Empty)) }));

            var actual = await _sut.GetAllAsync();

            Assert.Single(actual.Rules);
            Assert.IsType<HttpRuleResponse>(actual);
        }

        [Fact]
        public async Task DeleteAllHttpRules()
        {
            await _sut.RemoveAllAsync();

            _mockHttpRuleRepository.Verify(m => m.RemoveAllAsync());
        }
    }
}
