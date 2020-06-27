using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Application;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Mocker.Functions
{
    public class MockFunction
    {
        private readonly IHttpMockEngine _httpMockEngine;

        public MockFunction(IHttpMockEngine httpMockEngine)
        {
            _httpMockEngine = httpMockEngine;
        }

        [FunctionName(nameof(MockWithRoute))]
        public async Task<HttpResponseMessage> MockWithRoute(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "mock/{route}")] HttpRequest req,
            string route, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessSomething(req, route);
        }

        [FunctionName(nameof(Mock))]
        public async Task<HttpResponseMessage> Mock(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "mock")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await ProcessSomething(req, null);
        }

        private async Task<HttpResponseMessage> ProcessSomething(HttpRequest req, string? route)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var httpRequestDetails = new HttpRequestDetails(new HttpMethod(req.Method), route, body,
                new Dictionary<string, string>(), req.GetQueryParameterDictionary());

            var mockResponse = _httpMockEngine.Process(httpRequestDetails);

            var response = new HttpResponseMessage(mockResponse.StatusCode)
            {
                Content = new StringContent(mockResponse.Body)
            };
            return response;
        }

        //private static NameValueCollection ConvertQueryStringToDictionary(QueryString queryString)
        //{
        //    return HttpUtility.ParseQueryString(queryString.ToString());
        //}
    }
}
