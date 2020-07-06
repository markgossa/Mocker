using Microsoft.Azure.Cosmos.Table;
using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

            var result = await FindByMethodAsync(httpRequestDetails.Method);
            var result2 = await FindAsync(new HttpMockHistoryFilter(httpRequestDetails.Method, httpRequestDetails.Route, httpRequestDetails.Body));
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

        public async Task<List<HttpRequestDetails>> FindByMethodAsync(HttpMethod httpMethod) => await Task.Run(() =>
        {
            return _table.CreateQuery<HttpRequestDetailsTableEntity>().Where(r =>
                r.Method == httpMethod.ToString())
            .Select(r => new HttpRequestDetails(new HttpMethod(r.Method), r.Route, r.Body, new Dictionary<string, List<string>>(), null,
                r.ReceivedTime))
            .ToList();
        });

        public async Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter) => await Task.Run(() =>
            {
                return _table.CreateQuery<HttpRequestDetailsTableEntity>().Where(r =>
                    r.Method == httpMockHistoryFilter.Method.ToString()
                    && r.Route == (httpMockHistoryFilter.Route ?? string.Empty)
                    && r.Body == (httpMockHistoryFilter.Body ?? string.Empty)
                    && r.ReceivedTime > DateTime.UtcNow.Add(-httpMockHistoryFilter.TimeFrame))
                .Select(r => new HttpRequestDetails(new HttpMethod(r.Method), r.Route, r.Body, new Dictionary<string, List<string>>(), 
                    null, r.ReceivedTime))
                .ToList();
            });
    }
}
