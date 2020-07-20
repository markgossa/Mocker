using Mocker.Functions.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TechTalk.SpecFlow;
using Xunit;

namespace Mocker.Functions.Tests.Integration
{
    [Binding]
    public class MockerAdminHttpHistorySteps
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _mockerAdminHttpHistoryUrl;
        private readonly Uri _mockerBaseUrl;
        private List<HttpHistoryItem> _httpHistory;

        public MockerAdminHttpHistorySteps(AppSettingsHelper settings)
        {
            _httpClient = new HttpClient();
            _mockerBaseUrl = new Uri(settings.MockerBaseUrl);
            _mockerAdminHttpHistoryUrl = new Uri($"{settings.MockerBaseUrl}/mockeradmin/http/history");
        }

        [Given(@"There is no HTTP history")]
        public async Task ThereisnoHTTPhistory() => await _httpClient.DeleteAsync(_mockerAdminHttpHistoryUrl);

        [Given(@"I have sent (.*) to the HTTP mock using the (.*) HTTP method (.*) times")]
        public async Task GivenIHaveCalledTheHttpMockAsync(string data, string method, int count)
        {
            for (var i = 0; i < count; i++)
            {
                await SendRequest(data, method, null, null);
            }
        }

        [When(@"I query for those (.*) requests by HTTP method")]
        public async Task WhenIQueryForThatRequestByHTTPMethod(string method)
        {
            var query = new NameValueCollection
            {
                { "method", method }
            };

            await QueryForRequests(query);
        }

        [When(@"I query for those (.*) requests by HTTP method and body (.*)")]
        public async Task WhenIQueryForThatRequestByHTTPMethod(string method, string body)
        {
            var query = new NameValueCollection
            {
                { "method", method },
                { "body", body }
            };

            await QueryForRequests(query);
        }

        [Then(@"the result should have (.*) requests")]
        public void ThenTheResultShouldBeReturnedWithTheCorrectCount(int expectedCount) =>
            Assert.Equal(expectedCount, _httpHistory.Count);

        [Given(@"I have sent a (.*) request to the HTTP mock with header key (.*) and value (.*)")]
        public async Task GivenIHaveSentARequestToTheHTTPMockWithHeaderValue(string method, string headerKey, string headerValue) =>
            await SendRequest("hello world!", method, headerKey, headerValue);

        [When(@"I query for that request by (.*) method and header key (.*) and value (.*)")]
        public async Task WhenIQueryForThatRequestByHTTPMethodAndHeaderValueAsync(string method, string headerKey, string headerValue)
        {
            var headersQuery = $"{headerKey}={headerValue},Host={_mockerAdminHttpHistoryUrl.Host}:{_mockerAdminHttpHistoryUrl.Port}";

            var query = new NameValueCollection
            {
                { "method", method },
                { "headers", headersQuery }
            };

            await QueryForRequests(query);
        }

        [Then(@"the result should have one (.*) request with header key (.*) and value (.*)")]
        public void ThenTheResultShouldHaveOneRequestWithHeaderKeyAndValue(string method, string headerKey, string headerValue)
        {
            Assert.Single(_httpHistory);
            Assert.Equal(method, _httpHistory.First().Method);
            Assert.Equal(headerValue, _httpHistory.First().Headers[headerKey][0]);
        }

        [Given(@"I have made a (.*) HTTP request (.*) times to route (.*)")]
        public async Task GivenIHaveMadeAHTTPRequestTimesToRoute(string method, int count, string route)
        {
            for (var i = 0; i < count; i++)
            {
                await SendRequest("hello world!", method, null, null, route);
            }
        }

        [When(@"I query for those (.*) requests by HTTP method and route (.*)")]
        public async Task WhenIQueryForThoseRequestsByHTTPMethodAndRoute(string method, string route)
        {
            var query = new NameValueCollection
            {
                { "method", method },
                { "route", route }
            };

            await QueryForRequests(query);
        }

        [Then(@"the result should correct timestamp data")]
        public void ThenTheResultShouldCorrectTimestampData()
        {
            foreach (var httpRequest in _httpHistory)
            {
                Assert.InRange(httpRequest.Timestamp, DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow);
            }
        }

        private async Task SendRequest(string data, string method, string headerKey, string headerValue,
            string route = null)
        {
            var url = new Uri(_mockerBaseUrl, $"{route}" ?? null);

            var request = new HttpRequestMessage(new HttpMethod(method), url);

            if (!string.IsNullOrWhiteSpace(headerKey) && !string.IsNullOrWhiteSpace(headerValue))
            {
                request.Headers.Add(headerKey, new List<string>() { headerValue });
            }

            if (method != HttpMethod.Delete.ToString() && method != HttpMethod.Get.ToString())
            {
                request.Content = new StringContent(data, Encoding.UTF8);
            }

            await _httpClient.SendAsync(request);
        }

        private async Task QueryForRequests(NameValueCollection query)
        {
            var queryString = ConvertToQueryString(query);

            var uri = new UriBuilder(_mockerAdminHttpHistoryUrl)
            {
                Query = queryString.ToString()
            };

            var response = await _httpClient.GetAsync(uri.ToString());
            var json = await response.Content.ReadAsStringAsync();
            _httpHistory = JsonSerializer.Deserialize<List<HttpHistoryItem>>(json);
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
