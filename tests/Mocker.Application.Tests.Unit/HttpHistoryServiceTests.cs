using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpHistoryServiceTests
    {
        private const string route = "route1";
        private const string body = "body1";
        private readonly HttpMethod method = HttpMethod.Get;

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
            var httpRequestDetails = new HttpRequestDetails(HttpMethod.Get, route, body, headers, queryString);
            await _sut.AddAsync(httpRequestDetails);

            _mockHttpHistoryRepository.Verify(m => m.AddAsync(It.Is<HttpRequestDetails>(h => h == httpRequestDetails)));
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodRouteAndBody()
        {
            var expected = BuildHttpRequestDetailsList();
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(expected));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(method, route, body, TimeSpan.FromSeconds(30));
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Equal(expected[0].Method, actual[0].Method);
            Assert.Equal(expected[0].Route, actual[0].Route);
            Assert.Equal(expected[0].Body, actual[0].Body);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodOnly()
        {
            var expected = BuildHttpRequestDetailsList();
            _mockHttpHistoryRepository.Setup(m => m.FindByMethodAsync(It.IsAny<HttpMethod>()))
                .Returns(Task.FromResult(expected));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(method, null, null, TimeSpan.FromSeconds(30));
            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindByMethodAsync(It.Is<HttpMethod>(h => h == method)));

            Assert.Equal(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Method, actual[i].Method);
                Assert.Equal(expected[i].Route, actual[i].Route);
                Assert.Equal(expected[i].Body, actual[i].Body);
            }
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodRouteAndBodyDoesNotThrowIfNoneFound()
        {
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(method, route, body, TimeSpan.FromSeconds(30));
            var actual = await _sut.FindAsync(httpMockHistoryFilter);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsByMethodOnlyDoesNotThrowIfNoneFound()
        {
            _mockHttpHistoryRepository.Setup(m => m.FindByMethodAsync(It.IsAny<HttpMethod>()))
                .Returns(Task.FromResult(new List<HttpRequestDetails>()));

            var httpMockHistoryFilter = new HttpMockHistoryFilter(method, null, null, TimeSpan.FromSeconds(30));
            var actual = await _sut.FindAsync(httpMockHistoryFilter);
        }

        [Fact]
        public async Task FindsHttpRequestDetailsBasedOnTimeSpanOldRequest()
        {
            var httpMockHistoryFilter = new HttpMockHistoryFilter(method, route, body, TimeSpan.FromMilliseconds(10));

            var expected = BuildHttpRequestDetailsList();
            _mockHttpHistoryRepository.Setup(m => m.FindAsync(It.IsAny<HttpMockHistoryFilter>()))
                .Returns(Task.FromResult(expected));

            await Task.Delay(20);

            var actual = await _sut.FindAsync(httpMockHistoryFilter);

            _mockHttpHistoryRepository.Verify(m => m.FindAsync(It.Is<HttpMockHistoryFilter>(h => h == httpMockHistoryFilter)));

            Assert.Empty(actual);
        }

        [Fact]
        public async Task DeletesAllHttpRequestDetails()
        {
            _mockHttpHistoryRepository.Setup(m => m.DeleteAllAsync());

            await _sut.DeleteAllAsync();

            _mockHttpHistoryRepository.Verify(m => m.DeleteAllAsync());
        }

        private List<HttpRequestDetails> BuildHttpRequestDetailsList() => new List<HttpRequestDetails>()
            {
                new HttpRequestDetails(method, null, null, new Dictionary<string, List<string>>(),
                    new Dictionary<string, string>()),
                new HttpRequestDetails(method, route, body, new Dictionary<string, List<string>>(),
                    new Dictionary<string, string>())
            };
    }
}
