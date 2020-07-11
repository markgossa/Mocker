using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
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
            builder.Services.AddSingleton<IHttpMockEngine, HttpMockEngine>();
            builder.Services.AddSingleton<IHttpHistoryService, HttpHistoryService>();
            builder.Services.AddSingleton<IHttpRequestProcessor, HttpRequestProcessor>();
            builder.Services.AddSingleton<IHttpRuleRepository, InMemoryHttpRuleRepository>();
            builder.Services.AddSingleton<IHistoryQueryProcessor, HistoryQueryProcessor>();
            builder.Services.AddSingleton<IHttpMockHistoryRepository, HttpMockHistoryTableRepository>();
            builder.Services.AddSingleton<IMapper<HttpRequestObject, Task<HttpRequestDetails>>, HttpRequestDetailsMapper>();
            builder.Services.AddSingleton(typeof(CloudTableClient), BuildCloudTableClient());
        }

        private CloudTableClient BuildCloudTableClient()
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            return storageAccount.CreateCloudTableClient();
        }
    }
}
