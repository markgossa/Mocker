using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Mapper.Functions.Tests.Unit
{
    public class HistoryRequestProcessorTests
    {
        private readonly HistoryRequestProcessor _sut;
        private const string _body = "hello world!";
        private const string _route = "api";
        private const string _timeframe = "00:00:10";
        private readonly Mock<IHttpMockHistoryService> _mockHttpMockHistoryService = new Mock<IHttpMockHistoryService>();

        public HistoryRequestProcessorTests()
        {
            _sut = new HistoryRequestProcessor(_mockHttpMockHistoryService.Object);
        }

        [Fact]
        public async Task ReturnsBadResponseIfNoMethodPassed()
        {
            var query = new Dictionary<string, string>()
            {
                { "hello", "world" }
            };

            var actual = await _sut.ProcessAsync(query);

            Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.Equal("Please pass a valid HTTP method to search for", await actual.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ReturnsBadResponseIfInvalidMethodPassed()
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "world" }
            };

            var actual = await _sut.ProcessAsync(query);

            Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.Equal("Please pass a valid HTTP method to search for", await actual.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ReturnsBadResponseIfInvalidTimeframePassed()
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "timeframe", "invalid" }
            };

            var actual = await _sut.ProcessAsync(query);

            Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.Equal("Please pass a valid timeframe to search for", await actual.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ReturnsOkResponseIfNoTimeframePassed()
        {
            var query = new Dictionary<string, string>()
            {
                { "method", "get" }
            };

            var actual = await _sut.ProcessAsync(query);

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

            var actual = await _sut.ProcessAsync(query);

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task CallsHttpMockHistoryServiceWithMethodOnly()
        {
            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
            .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var query = new Dictionary<string, string>()
            {
                { "method", "get" }
            };

            await _sut.ProcessAsync(query);

            _mockHttpMockHistoryService.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(
                f => f.Method == HttpMethod.Get
                && f.Body == null
                && f.Route == null)));
        }

        [Fact]
        public async Task CallsHttpMockHistoryServiceWithMethodBodyAndRoute()
        {
            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
            .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "route", _route },
                { "body", _body }
            };

            await _sut.ProcessAsync(query);

            _mockHttpMockHistoryService.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(
                f => f.Method == HttpMethod.Get
                && f.Body == _body
                && f.Route == _route)));
        }

        [Fact]
        public async Task CallsHttpMockHistoryServiceWithTimeFrameIfSpecified()
        {
            _mockHttpMockHistoryService.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
            .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var query = new Dictionary<string, string>()
            {
                { "method", "get" },
                { "timeframe", _timeframe}
            };

            await _sut.ProcessAsync(query);

            _mockHttpMockHistoryService.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(
                f => f.TimeFrame == TimeSpan.Parse(_timeframe))));
        }
    }
}
