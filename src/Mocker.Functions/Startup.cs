using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Mocker.Application;
using Mocker.Functions;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using Mocker.Infrastructure;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Mocker.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IHttpMockEngine, HttpMockEngine>();
            builder.Services.AddSingleton<IHttpRequestProcessor, HttpRequestProcessor>();
            builder.Services.AddSingleton<IMockHttpRuleRepository, InMemoryHttpMockRuleRepository>();
            builder.Services.AddSingleton<IMapper<HttpRequestObject, Task<HttpRequestDetails>>, HttpRequestDetailsMapper>();
        }
    }
}
