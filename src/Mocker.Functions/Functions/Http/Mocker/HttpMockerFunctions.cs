using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Functions.Http.Mocker
{
    public class HttpMockerFunctions
    {
        private readonly IHttpRequestProcessor _httpRequestProcessor;

        public HttpMockerFunctions(IHttpRequestProcessor httpMockEngine)
        {
            _httpRequestProcessor = httpMockEngine;
        }

        [FunctionName(nameof(HttpMocker))]
        public async Task<HttpResponseMessage> HttpMocker(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", "head", "options",
            "patch", "trace", Route = "{route?}")] HttpRequest request,
            string route, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessHttpRequest(request, $"api/{route}");
        }

        private async Task<HttpResponseMessage> ProcessHttpRequest(HttpRequest req, string? route)
        {
            var httpRequestObject = new HttpRequestObject(req.Body, new HttpMethod(req.Method),
                new Dictionary<string, string>(req.GetQueryParameterDictionary()), req.Headers, route);

            return await _httpRequestProcessor.ProcessAsync(httpRequestObject);
        }
    }
}
