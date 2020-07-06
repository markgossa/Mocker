using Mocker.Domain.Models.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpRequestDetailsTests
    {
        private const string _route = "route1";
        private const string _body = "Hello world!";

        private readonly HttpMethod _httpMethod = HttpMethod.Post;
        private readonly Dictionary<string, List<string>> _headers = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, string> _queryString = new Dictionary<string, string>()
            {
                { "name", "mark" }
            };

        [Fact]
        public void CreateValidHttpRequestDetails()
        {
            var actual = new HttpRequestDetails(_httpMethod, _route, _body,
                _headers, _queryString);

            Assert.Equal(_httpMethod, actual.Method);
            Assert.Equal(_route, actual.Route);
            Assert.Equal(_body, actual.Body);
            Assert.Equal(_headers, actual.Headers);
            Assert.Equal(_queryString, actual.Query);
            Assert.InRange(actual.Timestamp, DateTime.UtcNow.AddMilliseconds(-10), DateTime.UtcNow);
        }

        [Fact]
        public void CreateValidHttpRequestDetailsWithDefinedTimestamp()
        {
            var actual = new HttpRequestDetails(_httpMethod, _route, _body,
                _headers, _queryString, DateTime.UtcNow.AddDays(-1));

            Assert.Equal(_httpMethod, actual.Method);
            Assert.Equal(_route, actual.Route);
            Assert.Equal(_body, actual.Body);
            Assert.Equal(_headers, actual.Headers);
            Assert.Equal(_queryString, actual.Query);
            Assert.InRange(actual.Timestamp, DateTime.UtcNow.AddDays(-1).AddMilliseconds(-10), DateTime.UtcNow.AddDays(-1));
        }
    }
}
