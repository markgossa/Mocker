using Microsoft.Azure.Cosmos.Table;
using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Xunit;

namespace Mocker.Infrastructure.Tests.Unit
{
    public class HttpRequestDetailsTableEntityTests
    {
        private const string _body = "body1";
        private const string _route = "route1";
        private readonly Dictionary<string, List<string>> _headers = new Dictionary<string, List<string>>()
            {
                { "header1", new List<string>(){ "value1", "value2" }
},
                { "header2", new List<string>(){ "value1", "value2" } }
            };

        private readonly Dictionary<string, string> _query = new Dictionary<string, string>()
            {
                { "key1", "value1" }
            };
        private readonly HttpMethod _method = HttpMethod.Get;

        [Fact]
        public void HttpRequestDetailsTableEntityImplementsTableEntity()
        {
            var actual = new HttpRequestDetailsTableEntity();

            Assert.True(actual is TableEntity);
        }

        [Fact]
        public void CreatesHttpRequestDetailsTableEntityWithPartitionKeyAndRowKey()
        {
            var actual = BuildHttpRequestDetailsTableEntity();

            Assert.True(Guid.TryParse(actual.PartitionKey, out _));
            Assert.True(Guid.TryParse(actual.RowKey, out _));
        }

        [Fact]
        public void CreatesHttpRequestDetailsTableEntityWithCorrectSimpleProperties()
        {
            var actual = BuildHttpRequestDetailsTableEntity();

            Assert.Equal(_method.ToString(), actual.Method);
            Assert.Equal(_route, actual.Route);
            Assert.Equal(_body, actual.Body);
            Assert.InRange(actual.ReceivedTime, DateTime.UtcNow.AddMilliseconds(-10), DateTime.UtcNow);
        }

        [Fact]
        public void CreatesHttpRequestDetailsTableEntityWithCorrectJsonSerializedProperties()
        {
            var actual = BuildHttpRequestDetailsTableEntity();

            Assert.Equal(JsonSerializer.Serialize(_headers), actual.Headers);
            Assert.Equal(JsonSerializer.Serialize(_query), actual.Query);
        }

        [Fact]
        public void CreatesHttpRequestDetailsTableEntityWithNullProperties()
        {
            var actual = BuildHttpRequestDetailsTableEntityWithHeadersAndMethod();

            Assert.Equal(_method.ToString(), actual.Method);
            Assert.Null(actual.Route);
            Assert.Null(actual.Body);
            Assert.Equal(JsonSerializer.Serialize(_headers), actual.Headers);
            Assert.Equal("null", actual.Query);
            Assert.InRange(actual.ReceivedTime, DateTime.UtcNow.AddMilliseconds(-10), DateTime.UtcNow);
        }

        private HttpRequestDetailsTableEntity BuildHttpRequestDetailsTableEntity() => 
            new HttpRequestDetailsTableEntity(
                new HttpRequestDetails(_method, _route, _body, _headers, _query));

        private HttpRequestDetailsTableEntity BuildHttpRequestDetailsTableEntityWithHeadersAndMethod() =>
            new HttpRequestDetailsTableEntity(
                new HttpRequestDetails(_method, null, null, _headers, null));
    }
}
