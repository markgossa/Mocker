using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Application.Contracts;
using Mocker.Functions.Contracts;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Functions.Http.MockerAdmin
{
    public class HttpMockerAdminHistoryFunctions
    {
        private readonly IHttpHistoryQueryService _historyRequestProcessor;
        private readonly IHttpHistoryService _httpHistoryService;

        public HttpMockerAdminHistoryFunctions(IHttpHistoryQueryService httpHistoryRequestProcessor,
            IHttpHistoryService httpHistoryService)
        {
            _historyRequestProcessor = httpHistoryRequestProcessor;
            _httpHistoryService = httpHistoryService;
        }

        [FunctionName(nameof(GetHttpHistory))]
        public async Task<HttpResponseMessage> GetHttpHistory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mockeradmin/http/history")] HttpRequest request, ILogger log)
        {
            log.LogInformation("MockerAdmin processed a request to query HTTP history");
            var query = request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

            return await _historyRequestProcessor.ExecuteQueryAsync(query);
        }

        [FunctionName(nameof(DeleteHttpHistory))]
        public async Task<IActionResult> DeleteHttpHistory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "mockeradmin/http/history")] HttpRequest request, ILogger log)
        {
            log.LogInformation("MockerAdmin processed a request to delete all HTTP history");
            await _httpHistoryService.DeleteAllAsync();

            return new OkResult();
        }
    }
}
