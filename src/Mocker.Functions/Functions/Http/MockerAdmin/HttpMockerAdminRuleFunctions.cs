using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocker.Functions.Functions.Http.MockerAdmin
{
    public class HttpMockerAdminRuleFunctions
    {
        private readonly IHttpRuleRequestProcessor _httpRuleRequestProcessor;

        public HttpMockerAdminRuleFunctions(IHttpRuleRequestProcessor httpRuleRequestProcessor)
        {
            _httpRuleRequestProcessor = httpRuleRequestProcessor;
        }

        [FunctionName(nameof(AddHttpRule))]
        public async Task<IActionResult> AddHttpRule(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "mockeradmin/http/rules")] HttpRequest request, ILogger log)
        {
            log.LogInformation("MockerAdmin processed a request to add an HTTP rule");

            var jsonRequest = await request.ReadAsStringAsync();
            var httpRuleRequest = JsonSerializer.Deserialize<HttpRuleRequest>(jsonRequest);
            await _httpRuleRequestProcessor.AddAsync(httpRuleRequest);

            return new OkResult();
        }

        //[FunctionName(nameof(GetHttpRules))]
        //public async Task<HttpResponseMessage> GetHttpRules(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mockeradmin/http/rules")] HttpRequest request, ILogger log)
        //{
        //    log.LogInformation("MockerAdmin processed a request to query HTTP rules");
        //    var query = request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

        //    return await _historyRequestProcessor.ExecuteQueryAsync(query);
        //}

    }
}
