using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocker.Functions.Functions.Mocks
{
    public class HistoryHttpFunctions
    {
        private readonly IHttpMockHistoryService _httpMockHistoryService;

        public HistoryHttpFunctions(IHttpMockHistoryService httpMockHistoryService)
        {
            _httpMockHistoryService = httpMockHistoryService;
        }

        [FunctionName(nameof(MockerAdminHistoryHttp))]
        public async Task<HttpResponseMessage> MockerAdminHistoryHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "MockerAdmin/history/http")] HttpRequest request, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
            
            return await ProcessQuery(query);
        }

        private async Task<HttpResponseMessage> ProcessQuery(Dictionary<string, string> query)
        {
            query.TryGetValue("method", out var value);
            var filterRequest = new HttpMockHistoryFilterRequest(new HttpMethod(value));

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(filterRequest, new ValidationContext(filterRequest), validationResults);

            var httpMockHistoryFilter = new HttpMockHistoryFilter(filterRequest.Method, null, null);

            var result = await _httpMockHistoryService.FindAsync(httpMockHistoryFilter);

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(result)),
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
