using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Application.Services;
using Mocker.Functions;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using Mocker.Functions.Services;
using Mocker.Infrastructure.Services;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Mocker.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IHttpMockEngine, HttpMockEngine>();
            builder.Services.AddSingleton<IHttpMockHistoryService, HttpMockHistoryService>();
            builder.Services.AddSingleton<IHttpRequestProcessor, HttpRequestProcessor>();
            builder.Services.AddSingleton<IHttpRuleRepository, InMemoryHttpRuleRepository>();
            builder.Services.AddSingleton<IHttpMockHistoryRepository, InMemoryHttpMockHistoryRepository>();
            builder.Services.AddSingleton<IMapper<HttpRequestObject, Task<HttpRequestDetails>>, HttpRequestDetailsMapper>();
        }
    }
}
