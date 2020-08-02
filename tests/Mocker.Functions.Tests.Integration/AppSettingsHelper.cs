using Microsoft.Extensions.Configuration;

namespace Mocker.Functions.Tests.Integration
{
    public class AppSettingsHelper
    {
        private readonly IConfigurationRoot _configuration;

        public string MockerBaseUrl => _configuration.GetSection("MockerBaseUrl").Value;

        public AppSettingsHelper()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("local.settings.json", true)
                .Build();
        }
    }
}
