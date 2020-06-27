using Mocker.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Mocker.Infrastructure.Tests.Unit
{
    public class HttpMockRepositoryTests
    {
        private readonly InMemoryMockRuleRepository _sut;

        public HttpMockRepositoryTests()
        {
            _sut = new InMemoryMockRuleRepository();
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

        //[Fact]
        //public void MockHttpRepositoryUpdatesMockHttpResponses()
        //{
        //    var httpMock = new HttpMock();
        //    _sut.Add(httpMock);

        //    var updatedHttpMock = new HttpMock(new HttpMockResponse(HttpStatusCode.OK, string.Empty));
        //    _sut.Update(updatedHttpMock);
        //    var actual = _sut.GetAllMocks();

        //    Assert.DoesNotContain(actual, m => m == httpMock);
        //}


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
        public void MockHttpRepositoryFindsHttpMockRulesByQueryString()
        {
            var queryString = new Dictionary<string, string>()
            {
                { "content-type", "application/json" },
                { "code", "password1" }
            };

            const string route = "route1";
            const string body = "Hello world!";
            var httpMock = new HttpMockRule(
                new HttpFilter(HttpMethod.Get, body, route, queryString),
                new HttpMockAction(HttpStatusCode.NotFound, "Can't find it!")
            );

            var queryString2 = new Dictionary<string, string>()
            {
                { "content-type", "application/json" },
                { "code", "password2" }
            };

            var httpMock2 = new HttpMockRule(
                new HttpFilter(HttpMethod.Get, body, route, queryString2),
                new HttpMockAction(HttpStatusCode.OK, "Found it!")
            );

            _sut.Add(httpMock);
            _sut.Add(httpMock2);

            var actual = _sut.Find(HttpMethod.Get, queryString, body, route);

            Assert.Single(actual);
            Assert.Contains(actual, m => m == httpMock);
        }
    }
}
