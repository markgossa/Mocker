using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions
{
    public class MockHttpFunctions
    {
        private readonly IHttpRequestProcessor _httpRequestProcessor;

        public MockHttpFunctions(IHttpRequestProcessor httpMockEngine)
        {
            _httpRequestProcessor = httpMockEngine;
        }

        [FunctionName(nameof(MockWithRoute))]
        public async Task<HttpResponseMessage> MockWithRoute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "mock/{route}")] HttpRequest request,
            string route, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessHttpRequest(request, route);
        }

        [FunctionName(nameof(Mock))]
        public async Task<HttpResponseMessage> Mock(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "mock")] HttpRequest request, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessHttpRequest(request);
        }

        private async Task<HttpResponseMessage> ProcessHttpRequest(HttpRequest req, string? route = null)
        {
            var httpRequestObject = new HttpRequestObject(req.Body, new HttpMethod(req.Method),
                new Dictionary<string, string>(req.GetQueryParameterDictionary()), route);

            return await _httpRequestProcessor.ProcessAsync(httpRequestObject);
        }
    }
}
