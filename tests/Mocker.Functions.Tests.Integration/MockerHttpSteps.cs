using Mocker.Functions.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
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

        [When(@"I add a query-based rule to the rule database which returns (.*)")]
        public async Task WhenIAddAQuery_BasedRuleToTheRuleDatabaseWhichReturns(string actionBody)
        {
            _mockerResponseBody = actionBody;

            var ruleRequest = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Query = new Dictionary<string, string>
                    {
                        { "name", "Mark" }
                    }
                },
                Action = new HttpRuleActionRequest
                {
                    Body = actionBody
                }
            };

            await AddNewRule(ruleRequest);
        }

        [When(@"I send a (.*) request which contains the filter query")]
        public async Task WhenISendARequestWhichContainsTheFilterQuery(string method)
        {
            var query = new NameValueCollection
            {
                { "name", "Mark" }
            };

            var queryString = ConvertToQueryString(query);

            var uri = new UriBuilder(_mockerBaseUrl)
            {
                Query = queryString.ToString()
            };

            var response = await _httpClient.GetAsync(uri.ToString());
            _mockerResponseBody = await response.Content.ReadAsStringAsync();
        }

        [When(@"I send a (.*) request to route (.*) with body (.*)")]
        public async Task WhenISendARequest(string method, string route, string body) => await SendRequestAsync(method, route, body);

        [Then(@"I should receive a response with (.*)")]
        public void ThenIShouldReceiveAResponseWith(string expectedBody) => Assert.Equal(expectedBody, _mockerResponseBody);

        [When(@"I add a header-based rule to the rule database which returns (.*)")]
        public async Task WhenIAddAHeader_BasedRuleToTheRuleDatabaseWhichReturns(string actionBody)
        {
            _mockerResponseBody = actionBody;

            var ruleRequest = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Headers = new Dictionary<string, List<string>>
                    {
                        { "header1", new List<string> { "value1" } },
                        { "header2", new List<string> { "value1" } }
                    }
                },
                Action = new HttpRuleActionRequest
                {
                    Body = actionBody
                }
            };

            await AddNewRule(ruleRequest);
        }

        [When(@"I send a (.*) request which contains the filter headers")]
        public async Task WhenISendARequestWhichContainsTheFilterHeaders(string method)
        {
            var headers = new Dictionary<string, List<string>>
            {
                { "header1", new List<string> { "value1" } },
                { "header2", new List<string> { "value1" } },
                { "header3", new List<string> { "value3" } }
            };

            await SendRequestAsync(method, null, "hello world23", headers);
        }

        private async Task AddNewRule(HttpRuleRequest ruleRequest)
        {
            var content = JsonSerializer.Serialize(ruleRequest);
            var httpContent = new StringContent(content);
            await _httpClient.PostAsync(_mockerAdminHttpRulesUrl, httpContent);
        }

        private async Task<HttpRequestMessage> SendRequestAsync(string method, string route, string body,
            Dictionary<string, List<string>> headers = null, Dictionary<string, string> query = null)
        {
            var url = _mockerBaseUrl;
            if (route != "null")
            {
                url = new Uri(_mockerBaseUrl, route);
            }



            var request = new HttpRequestMessage(new HttpMethod(method), url)
            {
                Content = new StringContent(body)
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            };

            var response = await _httpClient.SendAsync(request);
            _mockerResponseBody = await response.Content.ReadAsStringAsync();
            return request;
        }

        private static string ConvertToQueryString(NameValueCollection query)
        {
            var items = new List<string>();
            foreach (string name in query)
            {
                items.Add(string.Concat(name, "=", HttpUtility.UrlEncode(query[name])));
            }

            var queryString = string.Join("&", items.ToArray());
            return queryString;
        }
    }
}
