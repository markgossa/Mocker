using Mocker.Functions.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace Mocker.Functions.Tests.Integration
{
    [Binding]
    public sealed class MockerHttpSteps
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _mockerBaseUrl;
        private readonly Uri _mockerAdminHttpRulesUrl;
        private string _mockerResponseBody;

        public MockerHttpSteps(AppSettingsHelper settings)
        {
            _httpClient = new HttpClient();
            _mockerAdminHttpRulesUrl = new Uri($"{settings.MockerBaseUrl}/mockeradmin/http/rules");
            _mockerBaseUrl = new Uri(settings.MockerBaseUrl);
        }

        [When(@"I add a rule based on (.*) method into the rule database which returns (.*)")]
        public async Task WhenIAddARuleBasedOnMethodIntoTheRuleDatabase(string method, string actionBody)
        {
            _mockerResponseBody = actionBody;

            var ruleRequest = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Method = method
                },
                Action = new HttpRuleActionRequest
                {
                    Body = actionBody
                }
            };

            var content = JsonSerializer.Serialize(ruleRequest);
            using var httpContent = new StringContent(content);
            await _httpClient.PostAsync(_mockerAdminHttpRulesUrl, httpContent);
        }
        
        [When(@"I add a rule based on (.*) body into the rule database which returns (.*)")]
        public async Task WhenIAddARuleBasedOnBodyIntoTheRuleDatabase(string filterBody, string actionBody)
        {
            _mockerResponseBody = actionBody;

            var ruleRequest = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Body = filterBody
                },
                Action = new HttpRuleActionRequest
                {
                    Body = actionBody
                }
            };

            var content = JsonSerializer.Serialize(ruleRequest);
            using var httpContent = new StringContent(content);
            await _httpClient.PostAsync(_mockerAdminHttpRulesUrl, httpContent);
        }

        [When(@"I send a (.*) request with body (.*)")]
        public async Task WhenISendARequest(string method, string body)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), _mockerBaseUrl)
            {
                Content = new StringContent(body)
            };

            var response = await _httpClient.SendAsync(request);
            _mockerResponseBody = await response.Content.ReadAsStringAsync();
        }

        [Then(@"I should receive a response with (.*)")]
        public void ThenIShouldReceiveAResponseWith(string expectedBody) => Assert.Equal(expectedBody, _mockerResponseBody);
    }
}
