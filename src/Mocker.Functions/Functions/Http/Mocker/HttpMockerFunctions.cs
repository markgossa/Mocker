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
            [HttpTrigger(AuthorizationLevel.Anonymous, 
            Route = "{routeParam1:regex(^(?:(?!mockeradmin).)+$)?}/{routeParam2?}/{routeParam3?}/{routeParam4?}")] HttpRequest request,
            string? routeParam1, string? routeParam2, string? routeParam3, string? routeParam4, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var route = ConstructRoute(routeParam1, routeParam2, routeParam3, routeParam4);

            return await ProcessHttpRequest(request, $"{route}");
        }

        private string ConstructRoute(string? routeParam1, string? routeParam2, string? routeParam3, string? routeParam4)
        {
            var routePart1 = routeParam1 is null ? null : $"{routeParam1}";
            var routePart2 = routeParam2 is null ? null : $"/{routeParam2}";
            var routePart3 = routeParam3 is null ? null : $"/{routeParam3}";
            var routePart4 = routeParam4 is null ? null : $"/{routeParam4}";
            
            return $"{routePart1}{routePart2}{routePart3}{routePart4}";
        }

        private async Task<HttpResponseMessage> ProcessHttpRequest(HttpRequest req, string? route)
        {
            var httpRequestObject = new HttpRequestObject(req.Body, new HttpMethod(req.Method),
                new Dictionary<string, string>(req.GetQueryParameterDictionary()), req.Headers, route);

            return await _httpRequestProcessor.ProcessRequestAsync(httpRequestObject);
        }
    }
}
