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
    public class HttpMockFunctions
    {
        private readonly IHttpRequestProcessor _httpRequestProcessor;

        public HttpMockFunctions(IHttpRequestProcessor httpMockEngine)
        {
            _httpRequestProcessor = httpMockEngine;
        }

        [FunctionName(nameof(Mock))]
        public async Task<HttpResponseMessage> Mock(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "")] HttpRequest request, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessHttpRequest(request);
        }

        [FunctionName(nameof(MockWithRoute))]
        public async Task<HttpResponseMessage> MockWithRoute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{route}")] HttpRequest request,
            string route, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessHttpRequest(request, route);
        }

        private async Task<HttpResponseMessage> ProcessHttpRequest(HttpRequest req, string? route = null)
        {
            var httpRequestObject = new HttpRequestObject(req.Body, new HttpMethod(req.Method),
                new Dictionary<string, string>(req.GetQueryParameterDictionary()), req.Headers, route);

            return await _httpRequestProcessor.ProcessAsync(httpRequestObject);
        }
    }
}
