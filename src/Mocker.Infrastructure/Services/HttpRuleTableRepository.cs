using Microsoft.Azure.Cosmos.Table;
using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocker.Infrastructure.Services
{
    public class HttpRuleTableRepository : IHttpRuleRepository
    {
        private readonly CloudTable _table;
        private readonly List<HttpRule> _httpRuleCache;
        private int _nextHttpRuleId;

        public HttpRuleTableRepository(CloudTableClient tableClient)
        {
            _table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("HttpRuleTable") ?? "MockerHttpRule");
            _httpRuleCache = PopulateHttpRuleCache();
            _nextHttpRuleId = _table.CreateIfNotExists() ? 1 : _httpRuleCache.Count + 1;
        }

        public async Task AddAsync(HttpRule httpRule)
        {
            var newTableEntity = new HttpRuleTableEntity(httpRule, _nextHttpRuleId);
            var insertOperation = TableOperation.Insert(newTableEntity);
            await _table.ExecuteAsync(insertOperation);

            _httpRuleCache.Add(MapToHttpRule(newTableEntity));
            _nextHttpRuleId++;
        }

        public async Task RemoveAllAsync()
        {
            var allRows = _table.ExecuteQuery(new TableQuery<HttpRuleTableEntity>());
            var deleteTasks = new List<Task>();
            foreach (var row in allRows)
            {
                deleteTasks.Add(_table.ExecuteAsync(TableOperation.Delete(row)));
            }

            await Task.WhenAll(deleteTasks);

            _httpRuleCache.Clear();
            _nextHttpRuleId = 1;
        }

        public async Task<List<HttpRule>> GetAllAsync() => await Task.Run(() =>
        {
            return _httpRuleCache;
        });

        private List<HttpRule> PopulateHttpRuleCache() => _table.ExecuteQuery(new TableQuery<HttpRuleTableEntity>())
            .Select(r => MapToHttpRule(r)).OrderBy(r => r.Id).ToList();

        private static HttpRule MapToHttpRule(HttpRuleTableEntity httpRuleTableEntity)
        {
            HttpMethod? httpFilterMethod = null;
            if (httpRuleTableEntity.HttpFilterMethod != null)
            {
                httpFilterMethod = new HttpMethod(httpRuleTableEntity.HttpFilterMethod);
            }

            var httpFilter = new HttpFilter(httpFilterMethod, httpRuleTableEntity.HttpFilterBody, 
                httpRuleTableEntity.HttpFilterRoute, JsonSerializer.Deserialize<Dictionary<string, string>>(httpRuleTableEntity.HttpFilterQuery),
                JsonSerializer.Deserialize<Dictionary<string, List<string>>>(httpRuleTableEntity.HttpFilterHeaders));

            var httpAction = new HttpAction((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), httpRuleTableEntity.HttpActionStatusCode ?? "OK"),
                httpRuleTableEntity.HttpActionBody, JsonSerializer.Deserialize<Dictionary<string, List<string>>>(httpRuleTableEntity.HttpActionHeaders),
                httpRuleTableEntity.HttpActionDelay);

            return new HttpRule(httpFilter, httpAction, httpRuleTableEntity.Id);
        }
    }
}
