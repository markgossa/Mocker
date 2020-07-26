using Mocker.Functions.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TechTalk.SpecFlow;
using Xunit;
using Mocker.Application.Models;
using System.Linq;
using System.Diagnostics;

namespace Mocker.Functions.Tests.Integration
{
    [Binding]
    public sealed class MockerHttpSteps
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _mockerBaseUrl;
        private readonly Uri _mockerAdminHttpRulesUrl;
        private HttpResponseMessage _mockerResponse;
        private long _mockerResponseTime;

        public MockerHttpSteps(AppSettingsHelper settings)
        {
            _httpClient = new HttpClient();
            _mockerAdminHttpRulesUrl = new Uri($"{settings.MockerBaseUrl}/mockeradmin/http/rules");
            _mockerBaseUrl = new Uri(settings.MockerBaseUrl);
        }

        [When(@"I add a rule based on (.*) method into the rule database which returns (.*)")]
        public async Task WhenIAddARuleBasedOnMethodIntoTheRuleDatabase(string method, string actionBody)
        {
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

        [When(@"I add a rule filter on (.*) method and (.*) body into the rule database which returns (.*)")]
        public async Task WhenIAddARuleBasedOnBodyAndMethodIntoTheRuleDatabase(string filterMethod, string filterBody, string actionBody)
        {
            var ruleRequest = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Body = filterBody,
                    Method = filterMethod
                },
                Action = new HttpRuleActionRequest
                {
                    Body = actionBody
                }
            };

            await AddNewRule(ruleRequest);
        }

        [When(@"I send a request which contains the filter query")]
        public async Task WhenISendARequestWhichContainsTheFilterQuery()
        {
            var query = new NameValueCollection
            {
                { "name", "Mark" }
            };

            await SendRequestAsync("GET", null, null, null, query);
        }

        [When(@"I send a (.*) request to route (.*) with body (.*)")]
        public async Task WhenISendARequest(string method, string route, string body) => await SendRequestAsync(method, route, body);

        [Then(@"I should receive a response with (.*)")]
        public async Task ThenIShouldReceiveAResponseWith(string expectedBody) => Assert.Equal(expectedBody, await _mockerResponse.Content.ReadAsStringAsync());

        [When(@"I add a header-based rule to the rule database which returns (.*)")]
        public async Task WhenIAddAHeader_BasedRuleToTheRuleDatabaseWhichReturns(string actionBody)
        {
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

        [When(@"I add a complex rule which filters on method, body, headers, route and query with (.*) delay")]
        public async Task WhenIAddAComplexRuleWhichFiltersOnMethodBodyHeadersRouteAndQuery(int delay)
        {
            var ruleRequest = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Method = "POST",
                    Body = "Hello world!",
                    Query = new Dictionary<string, string>
                    {
                        { "query1", "value1" }
                    },
                    Route = "api/route22",
                    Headers = new Dictionary<string, List<string>>
                    {
                        { "header1", new List<string> { "value1" } },
                        { "header2", new List<string> { "value1" } }
                    }
                },
                Action = new HttpRuleActionRequest
                {
                    Body = "Hello back!",
                    Delay = delay,
                    Headers = new Dictionary<string, List<string>>
                    {
                        { "Result", new List<string> { "success" } }
                    },
                    StatusCode = HttpStatusCode.Accepted
                }
            };

            await AddNewRule(ruleRequest);
        }

        [Then(@"I should receive the default response")]
        public async Task ThenIShouldReceiveTheDefaultResponse()
        {
            Assert.Equal(HttpStatusCode.OK, _mockerResponse.StatusCode);
            Assert.Empty(await _mockerResponse.Content.ReadAsStringAsync());
        }

        [When(@"I send a matching complex request")]
        public async Task WhenISendAMatchingComplexRequest()
        {
            var headers = new Dictionary<string, List<string>>
            {
                { "header1", new List<string> { "value1" } },
                { "header2", new List<string> { "value1" } }
            };

            var query = new NameValueCollection
            {
                { "query1", "value1" }
            };

            await SendRequestAsync("POST", "api/route22", "Hello world!", headers, query);
        }

        [Then(@"I should receive the correct complex response with correct response properties with (.*) delay")]
        public async Task ThenIShouldReceiveTheCorrectResponseWithCorrectResponsePropertiesWithCorrectDelay(int delay)
        {
            var expectedHeaders = new Dictionary<string, List<string>>
            {
                { "Result", new List<string> { "success" } }
            };

            Assert.Equal(HttpStatusCode.Accepted, _mockerResponse.StatusCode);
            Assert.True(_mockerResponse.Headers.ToDictionary(x => x.Key, x => x.Value.ToList()).Contains(expectedHeaders));
            Assert.Equal("Hello back!", await _mockerResponse.Content.ReadAsStringAsync());
            Assert.InRange(_mockerResponseTime, delay - 200, delay + 200);
        }

        private async Task SendRequestAsync(string method, string route, string body,
            Dictionary<string, List<string>> headers = null, NameValueCollection query = null)
        {
            var uri = _mockerBaseUrl;
            if (route != "null")
            {
                uri = new Uri(_mockerBaseUrl, route);
            }

            if (query != null)
            {
                var uriBuilder = new UriBuilder(uri)
                {
                    Query = ConvertToQueryString(query).ToString()
                };
                uri = uriBuilder.Uri;
            }

            var request = new HttpRequestMessage(new HttpMethod(method), uri)
            {
                Content = new StringContent(body ?? string.Empty)
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            };

            var stopwatch = Stopwatch.StartNew();
            _mockerResponse = await _httpClient.SendAsync(request);
            stopwatch.Stop();
            _mockerResponseTime = stopwatch.ElapsedMilliseconds;
        }

        private string ConvertToQueryString(NameValueCollection query)
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
