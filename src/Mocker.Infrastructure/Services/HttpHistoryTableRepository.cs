using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocker.Infrastructure.Services
{
    public class HttpHistoryTableRepository : IHttpHistoryRepository
    {
        private readonly CloudTable _table;
        private readonly BlobContainerClient _blobContainerClient;

        public HttpHistoryTableRepository(CloudTableClient tableClient, BlobServiceClient blobClient)
        {
            _table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("HttpHistoryTable") ?? "MockerHttpHistory");
            _table.CreateIfNotExists();
            _blobContainerClient = blobClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("HttpHistoryContainer") ?? "mocker-http-history");
            _blobContainerClient.CreateIfNotExists();
        }

        public async Task AddAsync(HttpRequestDetails httpRequestDetails)
        {
            var requestDetailsTableEntity = new HttpRequestDetailsTableEntity(httpRequestDetails);
            await WriteBodyToBlobIfTooLarge(requestDetailsTableEntity);
            await AddRequestToTableAsync(requestDetailsTableEntity);
        }

        private async Task WriteBodyToBlobIfTooLarge(HttpRequestDetailsTableEntity requestDetailsTableEntity)
        {
            if (requestDetailsTableEntity.Body?.Length > 30000)
            {
                requestDetailsTableEntity.BodyBlobName = $"{Guid.NewGuid()}.txt";
                await AddBodyToBlobAsync(requestDetailsTableEntity.Body, requestDetailsTableEntity.BodyBlobName);

                requestDetailsTableEntity.Body = null;
            }
        }

        private async Task AddRequestToTableAsync(HttpRequestDetailsTableEntity requestDetailsTableEntity)
        {
            var insertOperation = TableOperation.Insert(requestDetailsTableEntity);
            await _table.ExecuteAsync(insertOperation);
        }

        private async Task AddBodyToBlobAsync(string body, string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(new MemoryStream(Encoding.Default.GetBytes(body)));
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
                .Where(r => r.Method == httpMockHistoryFilter.Method.ToString()
                    && r.ReceivedTime > DateTime.UtcNow.Add(-httpMockHistoryFilter.TimeFrame))
                .Select(r => new HttpRequestDetails(new HttpMethod(r.Method), r.Route, r.Body, DeserializeHeaders(r.Headers),
                    null, r.ReceivedTime))
                .ToList();
        });
    }
}
