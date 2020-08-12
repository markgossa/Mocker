using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Mapper.Functions.Tests.Unit
{
    public class HttpHistoryQueryServiceTests
    {
        private const string _body = "hello world!";
        private const string _route = "api";
        private const string _timeframe = "00:00:10";
        private readonly HttpHistoryQueryService _sut;
        private readonly Mock<IHttpHistoryService> _mockHttpMockHistoryService = new Mock<IHttpHistoryService>();

        public HttpHistoryQueryServiceTests()
        {
            _sut = new HttpHistoryQueryService(_mockHttpMockHistoryService.Object);
        }

        [Fact]
        public async Task ReturnsBadResponseIfNoMethod()
        {
            var query = new Dictionary<string, string>()
            {
                { "hello", "world" }
            };

            var actual = await _sut.ExecuteQueryAsync(query);

            Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.Equal("Please pass a valid HTTP method to search for", await actual.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ReturnsBadResponseIfInvalidMethod()
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "world" }
            };

            var actual = await _sut.ExecuteQueryAsync(query);

            Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.Equal("Please pass a valid HTTP method to search for", await actual.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ReturnsBadResponseIfInvalidTimeframe()
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "timeframe", "invalid" }
            };

            var actual = await _sut.ExecuteQueryAsync(query);

            Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.Equal("Please pass a valid timeframe to search for", await actual.Content.ReadAsStringAsync());
        }

        [Theory]
        [InlineData("header1=")]
        [InlineData("")]
        public async Task ReturnsBadResponseIfInvalidHeader(string headerData)
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "headers", headerData }
            };

            var actual = await _sut.ExecuteQueryAsync(query);

            Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.Equal("Please pass a valid header in the query string to search for e.g. header=key1=value1,key2=value2",
                await actual.Content.ReadAsStringAsync());
        }

        [Theory]
        [InlineData("header1")]
        [InlineData("key1=value1")]
        [InlineData("key1=value1,key2=value2")]
        public async Task ReturnsOkResponseWithValidHeaders(string headerData)
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "headers", headerData }
            };

            var actual = await _sut.ExecuteQueryAsync(query);

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task ReturnsOkResponseIfNoTimeframePassed()
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "get" }
            };

            var actual = await _sut.ExecuteQueryAsync(query);

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task ReturnsOkResponseIfValidTimeframePassed()
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "timeframe", "00:00:10" }
            };

            var actual = await _sut.ExecuteQueryAsync(query);

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task QueriesHttpMockHistoryServiceWithMethodOnly()
        {
            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var query = new Dictionary<string, string>()
            {
                { "method", "get" }
            };

            await _sut.ExecuteQueryAsync(query);

            _mockHttpMockHistoryService.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(
                f => f.Method == HttpMethod.Get
                && f.Body == null
                && f.Route == null)));
        }

        [Fact]
        public async Task QueriesHttpMockHistoryServiceWithMethodBodyAndRoute()
        {
            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "route", _route },
                { "body", _body }
            };

            await _sut.ExecuteQueryAsync(query);

            _mockHttpMockHistoryService.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(
                f => f.Method == HttpMethod.Get
                && f.Body == _body
                && f.Route == _route)));
        }

        [Fact]
        public async Task QueriesHttpMockHistoryServiceWithTimeFrame()
        {
            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "timeframe", _timeframe}
            };

            await _sut.ExecuteQueryAsync(query);

            _mockHttpMockHistoryService.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(
                f => f.TimeFrame == TimeSpan.Parse(_timeframe))));
        }

        [Fact]
        public async Task QueriesHttpMockHistoryServiceWithHeaders()
        {
            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var expectedHeaders = new Dictionary<string, List<string>>()
            {
                { "key1", new List<string>(){ "value1" } },
                { "key2", new List<string>(){ "value2" } }
            };

            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "headers", "key1=value1,key2=value2"}
            };

            await _sut.ExecuteQueryAsync(query);

            _mockHttpMockHistoryService.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(
                f => f.Headers["key1"][0] == "value1"
                && f.Headers["key2"][0] == "value2")));
        }

        [Fact]
        public async Task ReturnsHttpHistoryData()
        {
            var expectedBody1 = "hello world!";
            var expectedBody2 = "goodbye world!";
            var expectedRoute1 = "route66";
            var expectedRoute2 = "route53";
            var expectedHeaderValue = "value2";
            var expectedQueryValue = "value1";

            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(BuildHttpRequestDetailsList(expectedBody1, expectedBody2, expectedRoute1, expectedRoute2)));

            var query = new Dictionary<string, string>()
            {
                { "method", "post" },
                { "timeframe", _timeframe}
            };

            var response = await _sut.ExecuteQueryAsync(query);
            var responseData = await response.Content.ReadAsStringAsync();
            var actual = JsonSerializer.Deserialize<List<HttpHistoryItem>>(responseData);

            Assert.Equal(2, actual.Count);
            Assert.Equal(expectedBody1, actual[0].Body);
            Assert.Equal(expectedBody2, actual[1].Body);
            Assert.Equal(expectedRoute1, actual[0].Route);
            Assert.Equal(expectedRoute2, actual[1].Route);
            Assert.Equal(expectedHeaderValue, actual[0].Headers["header1"][0]);
            Assert.Equal(expectedHeaderValue, actual[1].Headers["header1"][0]);
            Assert.Equal(expectedQueryValue, actual[1].Query["query1"]);
            Assert.Equal(expectedQueryValue, actual[1].Query["query1"]);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private static List<HttpRequestDetails> BuildHttpRequestDetailsList(string body1, string body2,
            string route1, string route2)
        {
            var query = new Dictionary<string, string>()
            {
                { "query1", "value1" }
            };

            var headers = new Dictionary<string, List<string>>()
            {
                { "header1", new List<string>(){ "value2" } }
            };

            return new List<HttpRequestDetails>()
            {
                new HttpRequestDetails(HttpMethod.Post, route1, body1, headers, query),
                new HttpRequestDetails(HttpMethod.Post, route2, body2, headers, query)
            };
        }
    }
}
