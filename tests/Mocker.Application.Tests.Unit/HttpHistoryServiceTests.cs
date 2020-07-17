using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpHistoryServiceTests
    {
        private const string _route = "route1";
        private const string _body = "body1";
        private readonly HttpMethod _method = HttpMethod.Get;

        private readonly HttpHistoryService _sut;
        private readonly Mock<IHttpMockHistoryRepository> _mockHttpHistoryRepository;

        public HttpHistoryServiceTests()
        {
            _mockHttpHistoryRepository = new Mock<IHttpMockHistoryRepository>();
            _sut = new HttpHistoryService(_mockHttpHistoryRepository.Object);
        }

        [Fact]
        public async Task StoresHttpRequestDetailsInHttpRequestDetailsRepository()
        {
            var headers = new Dictionary<string, List<string>>();
            var queryString = new Dictionary<string, string>();
            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, _route, _body, headers, queryString);
            await _sut.AddAsync(httpRequestDetails);

            _mockHttpHistoryRepository.Verify(m => m.AddAsync(It.Is<HttpRequestDetails>(h => h == httpRequestDetails)));
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodOnly()
        {
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(BuildHttpRequestDetailsList()));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, null, null, null, null);
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Equal(2, actual.Count);
            Assert.Equal(_method, actual[0].Method);
            Assert.Equal(_route, actual[1].Route);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodRouteAndBody()
        {
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(BuildHttpRequestDetailsList()));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, _route, _body, TimeSpan.FromSeconds(30), null);
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Single(actual);
            Assert.Equal(_method, actual[0].Method);
            Assert.Equal(_route, actual[0].Route);
            Assert.Equal(_body, actual[0].Body);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodAndBody()
        {
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(BuildHttpRequestDetailsList()));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, null, _body, TimeSpan.FromSeconds(30), null);
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Single(actual);
            Assert.Equal(_method, actual[0].Method);
            Assert.Equal(_route, actual[0].Route);
            Assert.Equal(_body, actual[0].Body);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodAndJsonBody()
        {
            var testBody = new HttpRule(new HttpFilter(HttpMethod.Get, "hello world"),
                new HttpAction(HttpStatusCode.OK, "hi world"));
            var jsonBody = JsonSerializer.Serialize(testBody);
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(BuildHttpRequestDetailsList(null, null, jsonBody)));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, _route, jsonBody, TimeSpan.FromSeconds(30), null);
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Single(actual);
            Assert.Equal(_method, actual[0].Method);
            Assert.Equal(_route, actual[0].Route);
            Assert.Equal(jsonBody.Replace("\\", string.Empty), actual[0].Body);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodAndRoute()
        {
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(BuildHttpRequestDetailsList()));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, _route, null, TimeSpan.FromSeconds(30), null);
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Single(actual);
            Assert.Equal(_method, actual[0].Method);
            Assert.Equal(_route, actual[0].Route);
            Assert.Equal(_body, actual[0].Body);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsBySingleHeader()
        {
            var header1 = new Dictionary<string, List<string>>()
            {
                {"header1", new List<string>() { "hey!" } }
            };

            var headerToSearchFor = new Dictionary<string, List<string>>()
            {
                {"header1", new List<string>() { "hello world!" } }
            };

            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(BuildHttpRequestDetailsList(header1, headerToSearchFor)));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, null, null, TimeSpan.FromSeconds(30), headerToSearchFor);
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Single(actual);
            Assert.Equal(_method, actual[0].Method);
            Assert.Equal(_route, actual[0].Route);
            Assert.Equal(_body, actual[0].Body);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMultipleHeaders()
        {
            var header1 = new Dictionary<string, List<string>>()
            {
                {"header1", new List<string>() { "hey1!" } },
                {"header2", new List<string>() { "hey2!" } },
                {"header3", new List<string>() { "hey3!" } }
            };

            var headerToSearchFor = new Dictionary<string, List<string>>()
            {
                {"header1", new List<string>() { "hey1!" } },
                {"header2", new List<string>() { "hey2!" } },
                {"header3", new List<string>() { "hey3!" } },
                {"header4", new List<string>() { "hello world!" } }
            };

            var expected = BuildHttpRequestDetailsList(header1, headerToSearchFor);
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(expected));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, null, null, TimeSpan.FromSeconds(30), headerToSearchFor);
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Single(actual);
            Assert.Equal(_method, actual[0].Method);
            Assert.Equal(_route, actual[0].Route);
            Assert.Equal(_body, actual[0].Body);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsAndDoesNotThrowIfNoneFound()
        {
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(_method, _route, _body, TimeSpan.FromSeconds(30));
            var actual = await _sut.FindAsync(httpMockHistoryFilter);
        }

        [Fact]
        public async Task DeletesAllHttpRequestDetails()
        {
            _mockHttpHistoryRepository.Setup(m => m.DeleteAllAsync());

            await _sut.DeleteAllAsync();

            _mockHttpHistoryRepository.Verify(m => m.DeleteAllAsync());
        }

        private List<HttpRequestDetails> BuildHttpRequestDetailsList(
            Dictionary<string, List<string>>? header1 = null, Dictionary<string, List<string>>? header2 = null, 
            string? body = null) => new List<HttpRequestDetails>()
            {
                new HttpRequestDetails(_method, null, null, header1 ?? new Dictionary<string, List<string>>(),
                    new Dictionary<string, string>()),
                new HttpRequestDetails(_method, _route, body ?? _body, header2 ?? new Dictionary<string, List<string>>(),
                    new Dictionary<string, string>())
            };
    }
}
