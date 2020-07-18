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
    public class HttpMockerAdminRuleFunctions
    {
        //[FunctionName(nameof(GetHttpRules))]
        //public async Task<HttpResponseMessage> GetHttpRules(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mockeradmin/http/rules")] HttpRequest request, ILogger log)
        //{
        //    log.LogInformation("MockerAdmin processed a request to query HTTP rules");
        //    var query = request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

        //    return await _historyRequestProcessor.ExecuteQueryAsync(query);
        //}

        //[FunctionName(nameof(AddHttpRule))]
        //public async Task<IActionResult> AddHttpRule(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "mockeradmin/http/rules")] HttpRequest request, ILogger log)
        //{
        //    log.LogInformation("MockerAdmin processed a request to delete all HTTP history");
            

        //    return new OkResult();
        //}
    }
}
