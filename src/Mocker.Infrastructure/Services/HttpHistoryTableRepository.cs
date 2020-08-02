using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
            var addToTableAndBlobTasks = new List<Task>();
            if (requestDetailsTableEntity.Body?.Length > 30000)
            {
                requestDetailsTableEntity.BodyBlobName = $"{Guid.NewGuid()}.txt";
                addToTableAndBlobTasks.Add(AddBodyToBlobAsync(requestDetailsTableEntity.Body, requestDetailsTableEntity.BodyBlobName));
                requestDetailsTableEntity.Body = null;
            }
            addToTableAndBlobTasks.Add(AddRequestToTableAsync(requestDetailsTableEntity));
            await Task.WhenAll(addToTableAndBlobTasks);
        }

        private async Task AddBodyToBlobAsync(string body, string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            using var content = new MemoryStream(Encoding.Default.GetBytes(body));
            await blobClient.UploadAsync(content);
        }

        private async Task AddRequestToTableAsync(HttpRequestDetailsTableEntity requestDetailsTableEntity)
        {
            var insertOperation = TableOperation.Insert(requestDetailsTableEntity);
            await _table.ExecuteAsync(insertOperation);
        }

        public async Task DeleteAllAsync()
        {
            var allRows = _table.ExecuteQuery(new TableQuery<HttpRequestDetailsTableEntity>());
            var deleteTasks = new List<Task>();
            foreach (var row in allRows)
            {
                deleteTasks.Add(_table.ExecuteAsync(TableOperation.Delete(row)));
                if (row.BodyBlobName != null)
                {
                    deleteTasks.Add(_blobContainerClient.DeleteBlobIfExistsAsync(row.BodyBlobName));
                }
            }

            await Task.WhenAll(deleteTasks);
        }

        private Dictionary<string, List<string>> DeserializeHeaders(string? json) =>
            JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                ?? new Dictionary<string, List<string>>();

        public async Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter)
        {
            var httpRequestDetailEntities = _table.CreateQuery<HttpRequestDetailsTableEntity>()
                .Where(r => r.Method == httpMockHistoryFilter.Method.ToString()
                    && r.ReceivedTime > DateTime.UtcNow.Add(-httpMockHistoryFilter.TimeFrame));

            var httpRequestDetails = new List<HttpRequestDetails>();
            foreach (var entity in httpRequestDetailEntities)
            {
                var body = await GetRequestBody(entity);
                httpRequestDetails.Add(new HttpRequestDetails(new HttpMethod(entity.Method), entity.Route, body,
                    DeserializeHeaders(entity.Headers), null, entity.ReceivedTime));
            }

            return httpRequestDetails;
        }

        private async Task<string?> GetRequestBody(HttpRequestDetailsTableEntity requestDetails)
        {
            if (requestDetails.BodyBlobName != null)
            {
                return await DownloadBlobAsync(requestDetails.BodyBlobName);
            }

            return requestDetails.Body;
        }

        private async Task<string> DownloadBlobAsync(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            BlobDownloadInfo blob = await blobClient.DownloadAsync();
            using var stream = new MemoryStream();
            await blob.Content.CopyToAsync(stream);
            stream.Position = 0;
            using var streamReader = new StreamReader(stream);
            
            return await streamReader.ReadToEndAsync();
        }
    }
}
