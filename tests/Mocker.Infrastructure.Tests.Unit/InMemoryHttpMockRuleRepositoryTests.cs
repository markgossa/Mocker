using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mocker.Infrastructure.Tests.Unit
{
    public class InMemoryHttpMockRuleRepositoryTests
    {
        private readonly InMemoryHttpRuleRepository _sut;

        public InMemoryHttpMockRuleRepositoryTests()
        {
            _sut = new InMemoryHttpRuleRepository();
        }

        [Fact]
        public void MockHttpRepositorySavesMockHttpResponses()
        {
            var httpMock = BuildSimpleHttpRule();

            _sut.Add(httpMock);
            var actual = _sut.GetAll();

            Assert.True(actual.Count(m => m == httpMock) == 1);
        }

        [Fact]
        public void MockHttpRepositoryDeletesMockHttpResponses()
        {
            var httpMock = BuildSimpleHttpRule();
            _sut.Add(httpMock);
            _sut.Remove(httpMock);
            var actual = _sut.GetAll();

            Assert.DoesNotContain(actual, m => m == httpMock);
        }

        [Fact]
        public void MockHttpRepositoryFindsHttpMockRulesByMultipleParameters()
        {
            const string route = "route1";
            const string body = "Hello world!";
            var httpMock = new HttpRule(
                new HttpFilter(HttpMethod.Get, body, route, null),
                new HttpAction(HttpStatusCode.NotFound, "Can't find it!")
            );

            var httpMock2 = new HttpRule(
                new HttpFilter(HttpMethod.Get, "Hello London!", route, null),
                new HttpAction(HttpStatusCode.OK, "Found it!")
            );

            _sut.Add(httpMock);
            _sut.Add(httpMock2);

            var actual = _sut.Find(HttpMethod.Get, null, body, route);

            Assert.Single(actual);
            Assert.Contains(actual, m => m == httpMock);
        }

        [Fact]
        public void MockHttpRepositoryFindsHttpMockRulesByQuery()
        {
            var query = new Dictionary<string, string>()
            {
                { "content-type", "application/json" },
                { "code", "password1" }
            };

            const string route = "route1";
            const string body = "Hello world!";
            var httpMock = new HttpRule(
                new HttpFilter(HttpMethod.Get, body, route, query),
                new HttpAction(HttpStatusCode.NotFound, "Can't find it!")
            );

            var query2 = new Dictionary<string, string>()
            {
                { "content-type", "application/json" },
                { "code", "password2" }
            };

            var httpMock2 = new HttpRule(
                new HttpFilter(HttpMethod.Get, body, route, query2),
                new HttpAction(HttpStatusCode.OK, "Found it!")
            );

            _sut.Add(httpMock);
            _sut.Add(httpMock2);

            var actual = _sut.Find(HttpMethod.Get, query, body, route);

            Assert.Single(actual);
            Assert.Contains(actual, m => m == httpMock);
        }

        [Fact]
        public void MockHttpRepositoryReturnsEmptyListIfNoMatches()
        {
            var actual = _sut.Find(HttpMethod.Get, null, "hello world", null);

            Assert.Empty(actual);
        }

        private static HttpRule BuildSimpleHttpRule() => new HttpRule(new HttpFilter(HttpMethod.Get, string.Empty),
                        new HttpAction(HttpStatusCode.OK, string.Empty));
    }
}
