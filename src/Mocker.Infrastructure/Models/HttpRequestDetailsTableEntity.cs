using Microsoft.Azure.Cosmos.Table;
using Mocker.Domain.Models.Http;
using System;
using System.Text.Json;

namespace Mocker.Infrastructure.Models
{
    public class HttpRequestDetailsTableEntity : TableEntity
    {
        public string? Body { get; set; }
        public string? BodyBlobName { get; set; }
        public string? Headers { get; set; }
        public string? Method { get; set; }
        public string? Query { get; set; }
        public string? Route { get; set; }
        public DateTime ReceivedTime { get; set; }

        public HttpRequestDetailsTableEntity()
        {
        }

        public HttpRequestDetailsTableEntity(HttpRequestDetails httpRequestDetails)
        {
            PartitionKey = Guid.NewGuid().ToString();
            RowKey = Guid.NewGuid().ToString();
            Body = httpRequestDetails.Body;
            Headers = JsonSerializer.Serialize(httpRequestDetails.Headers);
            Method = httpRequestDetails.Method.ToString();
            Query = JsonSerializer.Serialize(httpRequestDetails.Query);
            Route = httpRequestDetails.Route;
            ReceivedTime = httpRequestDetails.Timestamp;
        }
    }
}
