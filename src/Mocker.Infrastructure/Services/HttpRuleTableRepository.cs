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

        public HttpRuleTableRepository(CloudTableClient tableClient)
        {
            _table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("HttpRuleTable"));
            _table.CreateIfNotExists();
        }

        public async Task AddAsync(HttpRule httpRule)
        {
            var insertOperation = TableOperation.Insert(new HttpRuleTableEntity(httpRule));
            await _table.ExecuteAsync(insertOperation);
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
        }

        public async Task<List<HttpRule>> GetAllAsync() => await Task.Run(() =>
        {
            return _table.ExecuteQuery(new TableQuery<HttpRuleTableEntity>())
            .Select(r =>
            {
                var httpFilter = new HttpFilter(new HttpMethod(r.HttpFilterMethod), r.HttpFilterBody, r.HttpFilterRoute,
                    JsonSerializer.Deserialize<Dictionary<string, string>>(r.HttpFilterQuery),
                    JsonSerializer.Deserialize<Dictionary<string, List<string>>>(r.HttpFilterHeaders), r.HttpFilterIgnoreHeaders);

                var httpAction = new HttpAction((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), r.HttpActionStatusCode ?? "OK"),
                    r.HttpActionBody, JsonSerializer.Deserialize<Dictionary<string, List<string>>>(r.HttpActionHeaders),
                    r.HttpActionDelay);

                return new HttpRule(httpFilter, httpAction);

            }).ToList();
        });

        public async Task<List<HttpRule>> FindAsync(HttpMethod httpMethod, string? body, string? route)
        {
            var results = await FindTableEntitiesAsync(httpMethod, body, route);
            return results.Select(r =>
            {
                var httpFilter = new HttpFilter(new HttpMethod(r.HttpFilterMethod), r.HttpFilterBody, r.HttpFilterRoute,
                    JsonSerializer.Deserialize<Dictionary<string, string>>(r.HttpFilterQuery),
                    JsonSerializer.Deserialize<Dictionary<string, List<string>>>(r.HttpFilterHeaders), r.HttpFilterIgnoreHeaders);

                var httpAction = new HttpAction((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), r.HttpActionStatusCode ?? "OK"),
                    r.HttpActionBody, JsonSerializer.Deserialize<Dictionary<string, List<string>>>(r.HttpActionHeaders),
                    r.HttpActionDelay);

                return new HttpRule(httpFilter, httpAction);
            }).ToList();
        }

        private async Task<List<HttpRuleTableEntity>> FindTableEntitiesAsync(HttpMethod httpMethod, string? body, string? route) => await Task.Run(() =>
        {
            var results = _table.CreateQuery<HttpRuleTableEntity>()
                .Where(r =>
                    r.HttpFilterMethod == httpMethod.ToString()
                    && r.HttpFilterBody == body
                    && r.HttpFilterRoute == route);

            return results.Select(r => r).ToList();
        });
    }
}
