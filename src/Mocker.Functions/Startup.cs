using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Mocker.Application.Contracts;
using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Mocker.Functions;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using Mocker.Infrastructure.Services;
using System;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Mocker.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IHttpRuleEngine, HttpRuleEngine>();
            builder.Services.AddSingleton<IHttpHistoryService, HttpHistoryService>();
            builder.Services.AddSingleton<IHttpRequestProcessor, HttpRequestProcessor>();
            builder.Services.AddSingleton<IHttpRuleRequestProcessor, HttpRuleRequestProcessor>();
            builder.Services.AddSingleton<IHttpRuleRepository, HttpRuleTableRepository>();
            builder.Services.AddSingleton<IHttpRuleService, HttpRuleService>();
            builder.Services.AddSingleton<IHttpHistoryQueryService, HttpHistoryQueryService>();
            builder.Services.AddSingleton<IHttpMockHistoryRepository, HttpMockHistoryTableRepository>();
            builder.Services.AddSingleton<IMapper<HttpRequestObject, Task<HttpRequestDetails>>, HttpRequestDetailsMapper>();
            builder.Services.AddSingleton(typeof(CloudTableClient), BuildCloudTableClient());
            //builder.Services.Configure<HttpOptions>(options => options.RoutePrefix = string.Empty);
        }

        private CloudTableClient BuildCloudTableClient()
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            return storageAccount.CreateCloudTableClient();
        }
    }
}
