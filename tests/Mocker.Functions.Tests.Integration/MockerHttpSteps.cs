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
            await AddNewRule(ruleRequest);
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

            await AddNewRule(ruleRequest);
        }

        [When(@"I add a rule based on (.*) route into the rule database which returns (.*)")]
        public async Task WhenIAddARuleBasedOnRouteIntoTheRuleDatabaseWhichReturns(string filterRoute, string actionBody)
        {
            _mockerResponseBody = actionBody;

            var ruleRequest = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Route = filterRoute
                },
                Action = new HttpRuleActionRequest
                {
                    Body = actionBody
                }
            };

            await AddNewRule(ruleRequest);
        }


        [When(@"I send a (.*) request to route (.*) with body (.*)")]
        public async Task WhenISendARequest(string method, string route, string body)
        {
            var url = _mockerBaseUrl;
            if (route != "null")
            {
                url = new Uri(_mockerBaseUrl, route);
            }

            using var request = new HttpRequestMessage(new HttpMethod(method), url)
            {
                Content = new StringContent(body)
            };

            var response = await _httpClient.SendAsync(request);
            _mockerResponseBody = await response.Content.ReadAsStringAsync();
        }

        [Then(@"I should receive a response with (.*)")]
        public void ThenIShouldReceiveAResponseWith(string expectedBody) => Assert.Equal(expectedBody, _mockerResponseBody);

        private async Task AddNewRule(HttpRuleRequest ruleRequest)
        {
            var content = JsonSerializer.Serialize(ruleRequest);
            var httpContent = new StringContent(content);
            await _httpClient.PostAsync(_mockerAdminHttpRulesUrl, httpContent);
        }
    }
}
