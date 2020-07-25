using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using System;
using System.Linq;
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
            HttpRuleRequest httpRuleRequest;
            try
            {
                httpRuleRequest = JsonSerializer.Deserialize<HttpRuleRequest>(jsonRequest, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            if (!_httpRuleRequestProcessor.Validate(httpRuleRequest, out var validationResults))
            {
                return new BadRequestObjectResult(validationResults.Select(x => x.ErrorMessage));
            }

            await _httpRuleRequestProcessor.AddAsync(httpRuleRequest);

            return new OkResult();
        }

        [FunctionName(nameof(GetAllHttpRules))]
        public async Task<IActionResult> GetAllHttpRules(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mockeradmin/http/rules")] HttpRequest request, ILogger log)
        {
            log.LogInformation("MockerAdmin processed a request to get all HTTP rules");

            return new OkObjectResult(await _httpRuleRequestProcessor.GetAllAsync());
        }

        [FunctionName(nameof(RemoveAllHttpRules))]
        public async Task<IActionResult> RemoveAllHttpRules(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "mockeradmin/http/rules")] HttpRequest request, ILogger log)
        {
            log.LogInformation("MockerAdmin processed a request to delete all HTTP rules");

            await _httpRuleRequestProcessor.RemoveAllAsync();

            return new OkResult();
        }
    }
}
