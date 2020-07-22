using Microsoft.Azure.Cosmos.Table;
using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Xunit;

namespace Mocker.Infrastructure.Tests.Unit
{
    public class HttpRuleTableEntityTests
    {
        private const string _filterBody = "hello world";
        private const string _filterRoute = "route1";
        private readonly Dictionary<string, string> _filterQuery = new Dictionary<string, string>
        {
            { "name", "mark" }
        };
        private readonly Dictionary<string, List<string>> _filterHeaders = new Dictionary<string, List<string>>
        {
            { "Content-Type", new List<string> { "application/json" } }
        };
        private readonly string _actionBody = "Response1";
        private readonly Dictionary<string, List<string>> _actionHeaders = new Dictionary<string, List<string>>
        {
            { "AccessType", new List<string> { "Type1" } }
        };
        private readonly HttpMethod _filterMethod = HttpMethod.Get;
        private const bool _filterIgnoreHeaders = false;
        private readonly HttpStatusCode _actionStatusCode = HttpStatusCode.OK;
        private readonly int _actionDelay = 500;

        [Fact]
        public void HttpRequestDetailsTableEntityImplementsTableEntity()
        {
            var actual = BuildHttpRuleTableEntity();

            Assert.True(actual is TableEntity);
        }

        [Fact]
        public void CreatesHttpRuleTableEntityWithPartitionKeyRowKeyAndId()
        {
            var actual = BuildHttpRuleTableEntity();

            Assert.True(Guid.TryParse(actual.PartitionKey, out _));
            Assert.True(Guid.TryParse(actual.RowKey, out _));
            Assert.Equal(1, actual.Id);
        }

        [Fact]
        public void CreatesHttpRuleTableEntityWithHttpFilterProperties()
        {
            var actual = BuildHttpRuleTableEntity();

            Assert.Equal(_filterMethod.ToString(), actual.HttpFilterMethod);
            Assert.Equal(_filterBody, actual.HttpFilterBody);
            Assert.Equal(_filterRoute, actual.HttpFilterRoute);
            Assert.Equal(JsonSerializer.Serialize(_filterQuery), actual.HttpFilterQuery);
            Assert.Equal(JsonSerializer.Serialize(_filterHeaders), actual.HttpFilterHeaders);
            Assert.Equal(_filterIgnoreHeaders, actual.HttpFilterIgnoreHeaders);
        }

        [Fact]
        public void CreatesHttpRuleTableEntityWithHttpActionProperties()
        {
            var actual = BuildHttpRuleTableEntity();

            Assert.Equal(_actionStatusCode.ToString(), actual.HttpActionStatusCode);
            Assert.Equal(_actionBody, actual.HttpActionBody);
            Assert.Equal(JsonSerializer.Serialize(_actionHeaders), actual.HttpActionHeaders);
            Assert.Equal(_actionDelay, actual.HttpActionDelay);
        }

        private HttpRuleTableEntity BuildHttpRuleTableEntity()
        {
            var httpFilter = new HttpFilter(_filterMethod, _filterBody, _filterRoute, _filterQuery, _filterHeaders, _filterIgnoreHeaders);
            var httpAction = new HttpAction(_actionStatusCode, _actionBody, _actionHeaders, _actionDelay);
            
            return new HttpRuleTableEntity(new HttpRule(httpFilter, httpAction), 1);
        }
    }
}
