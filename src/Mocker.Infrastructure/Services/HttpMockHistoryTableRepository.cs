using Microsoft.Azure.Cosmos.Table;
using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocker.Infrastructure.Services
{
    public class HttpMockHistoryTableRepository : IHttpMockHistoryRepository
    {
        private readonly CloudTable _table;

        public HttpMockHistoryTableRepository(CloudTableClient tableClient)
        {
            _table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("HttpMockHistoryTable"));
            _table.CreateIfNotExists();
        }

        public async Task AddAsync(HttpRequestDetails httpRequestDetails)
        {
            var insertOperation = TableOperation.Insert(new HttpRequestDetailsTableEntity(httpRequestDetails));
            await _table.ExecuteAsync(insertOperation);
        }

        public async Task DeleteAllAsync()
        {
            var allRows = _table.ExecuteQuery(new TableQuery<HttpRequestDetailsTableEntity>());
            var deleteTasks = new List<Task>();
            foreach (var row in allRows)
            {
                deleteTasks.Add(_table.ExecuteAsync(TableOperation.Delete(row)));
            }

            await Task.WhenAll(deleteTasks);
        }

        private Dictionary<string, List<string>> DeserializeHeaders(string? json) =>
            JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                ?? new Dictionary<string, List<string>>();

        public async Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter) => await Task.Run(() =>
        {
            return _table.CreateQuery<HttpRequestDetailsTableEntity>()
                .Where(r =>
                    r.Method == httpMockHistoryFilter.Method.ToString()
                    && r.ReceivedTime > DateTime.UtcNow.Add(-httpMockHistoryFilter.TimeFrame))
                //.Where(r => (string.IsNullOrWhiteSpace(httpMockHistoryFilter.Route) || r.Route == httpMockHistoryFilter.Route)
                //    && (string.IsNullOrWhiteSpace(httpMockHistoryFilter.Body) || r.Body == httpMockHistoryFilter.Body))
                .Select(r => new HttpRequestDetails(new HttpMethod(r.Method), r.Route, r.Body, DeserializeHeaders(r.Headers),
                    null, r.ReceivedTime))
                .ToList();
        });
    }
}
