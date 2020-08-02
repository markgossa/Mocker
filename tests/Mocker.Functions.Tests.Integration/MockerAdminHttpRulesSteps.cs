using Mocker.Functions.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace Mocker.Functions.Tests.Integration
{
    [Binding]
    public sealed class MockerAdminHttpRulesSteps
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _mockerAdminHttpRulesUrl;
        private readonly string _largeActionBody;

        public MockerAdminHttpRulesSteps(AppSettingsHelper settings)
        {
            _httpClient = new HttpClient();
            _mockerAdminHttpRulesUrl = new Uri($"{settings.MockerBaseUrl}/mockeradmin/http/rules");
            
            using var streamReader = new StreamReader("Data\\LargeActionBody.txt");
            _largeActionBody = streamReader.ReadToEnd();
        }

        [Given(@"There are no HTTP rules in the rules database")]
        public async Task GivenThereAreNoRulesInTheRulesDatabase() => await _httpClient.DeleteAsync(_mockerAdminHttpRulesUrl);

        [When(@"I add (.*) rules into the rule database")]
        public async Task WhenIAddRulesIntoTheRuleDatabase(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var ruleRequest = BuildRuleRequest(i);
                await AddRuleRequestAsync(ruleRequest);
            }
        }
        
        [Then(@"(.*) rule should exist in the rule database with correct settings")]
        public async Task ThenRuleShouldExistInTheRuleDatabase(int count)
        {
            var httpRuleResponse = await GetAllRules();
            for (var i = 0; i < count; i++)
            {
                Assert.Contains(httpRuleResponse.Rules, r =>
                {
                    return r.Filter.Body == $"Hello world{i}"
                        && r.Filter.Headers.TryGetValue("header1", out var filterHeaderValue)
                        && filterHeaderValue.First() == $"value{i}"
                        && r.Filter.Query.TryGetValue("name", out var filterQueryValue)
                        && filterQueryValue == $"mark{i}"
                        && r.Filter.Route == $"route{i}"
                        && r.Action.Body == $"Hey back{i}!"
                        && r.Action.Headers.TryGetValue("header1", out var actionHeaderValue)
                        && actionHeaderValue.First() == $"value{i}";
                });
            }
        }

        [When(@"I add (.*) rules into the rule database with large action body")]
        public async Task WhenIAddRulesIntoTheRuleDatabaseWithLargeActionBodyAsync(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var ruleRequest = new HttpRuleRequest()
                {
                    Filter = new HttpRuleFilterRequest(),
                    Action = new HttpRuleActionRequest()
                    {
                        Body = _largeActionBody
                    }
                };

                await AddRuleRequestAsync(ruleRequest);
            }
        }

        [Then(@"(.*) rules with large action body should exist in the rule database")]
        public async Task ThenRulesWithLargeActionBodyShouldExistInTheRuleDatabaseAsync(int count)
        {
            var httpRuleResponse = await GetAllRules();
            for (var i = 0; i < count; i++)
            {
                Assert.Contains(httpRuleResponse.Rules, r =>
                {
                    return r.Action.Body == _largeActionBody;
                });
            }
        }

        private async Task AddRuleRequestAsync(HttpRuleRequest ruleRequest)
        {
            var content = JsonSerializer.Serialize(ruleRequest);
            using var httpContent = new StringContent(content);
            await _httpClient.PostAsync(_mockerAdminHttpRulesUrl, httpContent);
        }

        private async Task<HttpRuleResponse> GetAllRules()
        {
            var response = await _httpClient.GetAsync(_mockerAdminHttpRulesUrl);
            var json = await response.Content.ReadAsStringAsync();
            var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var httpRuleResponse = JsonSerializer.Deserialize<HttpRuleResponse>(json, jsonSerializerOptions);
            return httpRuleResponse;
        }

        private HttpRuleRequest BuildRuleRequest(int index)
        {
            var expectedFilterBody = $"Hello world{index}";
            var expectedFilterHeaders = new Dictionary<string, List<string>>
            {
                { "header1", new List<string> { $"value{index}" } }
            };
            var expectedFilterMethod = "GET";
            var expectedFilterQuery = new Dictionary<string, string>
            {
                { "name", $"mark{index}" }
            };
            var expectedFilterRoute = $"route{index}";

            var expectedActionBody = $"Hey back{index}!";
            var expectedActionDelay = 500;
            var expectedActionStatusCode = HttpStatusCode.OK;
            var expectedActionHeaders = new Dictionary<string, List<string>>
            {
                { "header1", new List<string> { $"value{index}", "value2" } },
                { "header2", new List<string> { "value1", "value2" } }
            };

            return new HttpRuleRequest()
            {
                Filter = new HttpRuleFilterRequest()
                {
                    Body = expectedFilterBody,
                    Headers = expectedFilterHeaders,
                    Method = expectedFilterMethod,
                    Query = expectedFilterQuery,
                    Route = expectedFilterRoute
                },

                Action = new HttpRuleActionRequest()
                {
                    Body = expectedActionBody,
                    Delay = expectedActionDelay,
                    Headers = expectedActionHeaders,
                    StatusCode = expectedActionStatusCode
                }
            };
        }
    }
}
