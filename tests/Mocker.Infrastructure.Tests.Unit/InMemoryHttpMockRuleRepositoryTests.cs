using Mocker.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mocker.Infrastructure.Tests.Unit
{
    public class InMemoryHttpMockRuleRepositoryTests
    {
        private readonly InMemoryHttpMockRuleRepository _sut;

        public InMemoryHttpMockRuleRepositoryTests()
        {
            _sut = new InMemoryHttpMockRuleRepository();
        }

        [Fact]
        public void MockHttpRepositorySavesMockHttpResponses()
        {
            var httpMock = new HttpMockRule();
            _sut.Add(httpMock);
            var actual = _sut.GetAll();

            Assert.True(actual.Count(m => m == httpMock) == 1);
        }

        [Fact]
        public void MockHttpRepositoryDeletesMockHttpResponses()
        {
            var httpMock = new HttpMockRule();
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
            var httpMock = new HttpMockRule(
                new HttpFilter(HttpMethod.Get, body, route, null),
                new HttpMockAction(HttpStatusCode.NotFound, "Can't find it!")
            );

            var httpMock2 = new HttpMockRule(
                new HttpFilter(HttpMethod.Get, "Hello London!", route, null),
                new HttpMockAction(HttpStatusCode.OK, "Found it!")
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
            var httpMock = new HttpMockRule(
                new HttpFilter(HttpMethod.Get, body, route, query),
                new HttpMockAction(HttpStatusCode.NotFound, "Can't find it!")
            );

            var query2 = new Dictionary<string, string>()
            {
                { "content-type", "application/json" },
                { "code", "password2" }
            };

            var httpMock2 = new HttpMockRule(
                new HttpFilter(HttpMethod.Get, body, route, query2),
                new HttpMockAction(HttpStatusCode.OK, "Found it!")
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
    }
}
