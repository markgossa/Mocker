using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Cosmos.Table;
using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using Mocker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocker.Infrastructure.Services
{
    public class HttpRuleTableRepository : IHttpRuleRepository
    {
        private readonly CloudTable _table;
        private readonly List<HttpRuleTableEntity> _httpRuleTableEntityCache;
        private int _nextHttpRuleId;
        private readonly BlobContainerClient _blobContainerClient;

        public HttpRuleTableRepository(CloudTableClient tableClient, BlobServiceClient blobClient)
        {
            _table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("HttpRuleTable") ?? "MockerHttpRule");
            _httpRuleTableEntityCache = PopulateHttpRuleCache();
            _nextHttpRuleId = _table.CreateIfNotExists() ? 1 : _httpRuleTableEntityCache.Count + 1;

            _blobContainerClient = blobClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("HttpRuleContainer") ?? "mocker-http-rule");
            _blobContainerClient.CreateIfNotExists();
        }

        public async Task AddAsync(HttpRule httpRule)
        {
            var newTableEntity = new HttpRuleTableEntity(httpRule, _nextHttpRuleId);
            var addToTableAndBlobTasks = new List<Task>();
            if (newTableEntity.HttpActionBody?.Length > 30000)
            {
                newTableEntity.HttpActionBodyBlobName = $"{Guid.NewGuid()}.txt";
                addToTableAndBlobTasks.Add(AddBodyToBlobAsync(newTableEntity.HttpActionBody, newTableEntity.HttpActionBodyBlobName));
                newTableEntity.HttpActionBody = null;
            }

            addToTableAndBlobTasks.Add(AddRuleToTableAsync(newTableEntity));
            AddRuleToInMemoryCache(newTableEntity);
            await Task.WhenAll(addToTableAndBlobTasks);

            _nextHttpRuleId++;
        }

        public async Task RemoveAllAsync()
        {
            var allRows = _table.ExecuteQuery(new TableQuery<HttpRuleTableEntity>());
            var deleteTasks = new List<Task>();
            foreach (var row in allRows)
            {
                deleteTasks.Add(_table.ExecuteAsync(TableOperation.Delete(row)));
                if (row.HttpActionBodyBlobName != null)
                {
                    deleteTasks.Add(_blobContainerClient.DeleteBlobIfExistsAsync(row.HttpActionBodyBlobName));
                }
            }

            await Task.WhenAll(deleteTasks);

            _httpRuleTableEntityCache.Clear();
            _nextHttpRuleId = 1;
        }

        public async Task<IEnumerable<HttpRule>> GetCachedRulesAsync() => await Task.Run(() => _httpRuleTableEntityCache.Select(r => MapToHttpRule(r)));

        public async Task<HttpRule?> GetRuleDetailsAsync(int id)
        {
            var ruleEntity = _httpRuleTableEntityCache.FirstOrDefault(r => r.Id == id);
            return ruleEntity != null ? MapToHttpRule(await AddBlobActionBody(ruleEntity)) : null;
        }

        private List<HttpRuleTableEntity> PopulateHttpRuleCache() => _table.ExecuteQuery(new TableQuery<HttpRuleTableEntity>()).ToList();

        private async Task<HttpRuleTableEntity> AddBlobActionBody(HttpRuleTableEntity httpRuleTableEntity)
        {
            if (httpRuleTableEntity.HttpActionBodyBlobName != null)
            {
                httpRuleTableEntity.HttpActionBody = await DownloadBlobAsync(httpRuleTableEntity.HttpActionBodyBlobName);
            }

            return httpRuleTableEntity;
        }

        private async Task AddBodyToBlobAsync(string body, string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            using var content = new MemoryStream(Encoding.Default.GetBytes(body));
            await blobClient.UploadAsync(content);
        }

        private async Task AddRuleToTableAsync(HttpRuleTableEntity newTableEntity)
        {
            var insertOperation = TableOperation.Insert(newTableEntity);
            await _table.ExecuteAsync(insertOperation);
        }

        private void AddRuleToInMemoryCache(HttpRuleTableEntity newTableEntity) => _httpRuleTableEntityCache.Add(newTableEntity);

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
