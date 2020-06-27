using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Mocker.Application;
using Mocker.Functions;
using Mocker.Infrastructure;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Mocker.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IHttpMockEngine, HttpMockEngine>();
            builder.Services.AddSingleton<IMockRuleRepository, InMemoryMockRuleRepository>();
        }
    }
}
