using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Functions.Mocks
{
    public class MockerMockFunctions
    {
        private readonly IHttpRequestProcessor _httpRequestProcessor;

        public MockerMockFunctions(IHttpRequestProcessor httpMockEngine)
        {
            _httpRequestProcessor = httpMockEngine;
        }

        [FunctionName(nameof(Mock))]
        public async Task<HttpResponseMessage> Mock(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", "head", "options",
            "patch", "trace", Route = "{route?}")] HttpRequest request,
            string route, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessHttpRequest(request, route);
        }

        private async Task<HttpResponseMessage> ProcessHttpRequest(HttpRequest req, string? route)
        {
            var httpRequestObject = new HttpRequestObject(req.Body, new HttpMethod(req.Method),
                new Dictionary<string, string>(req.GetQueryParameterDictionary()), req.Headers, route);

            return await _httpRequestProcessor.ProcessAsync(httpRequestObject);
        }
    }
}
