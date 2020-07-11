using Mocker.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                if (method == "DELETE")
                {
                    await _httpClient.DeleteAsync(_mockerBaseUrl);
                }
                else if (method == "GET")
                {
                    await _httpClient.GetAsync(_mockerBaseUrl);
                }
                else if (method == "PATCH")
                {
                    await _httpClient.PatchAsync(_mockerBaseUrl, new StringContent(data));
                }
                else if (method == "POST")
                {
                    await _httpClient.PostAsync(_mockerBaseUrl, new StringContent(data));
                }
                else if (method == "PUT")
                {
                    await _httpClient.PutAsync(_mockerBaseUrl, new StringContent(data));
                }
            }
        }

        [When(@"I query for those (.*) requests by HTTP method")]
        public async Task WhenIQueryForThatRequestByHTTPMethod(string method)
        {
            var uri = new UriBuilder(_mockerAdminHttpHistoryUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["method"] = method;
            uri.Query = query.ToString();

            var response = await _httpClient.GetAsync(uri.ToString());
            var json = await response.Content.ReadAsStringAsync();
            _httpHistory = JsonSerializer.Deserialize<List<HttpHistoryItem>>(json);
        }

        [Then(@"the result should have (.*) (.*) requests with correct (.*)")]
        public void ThenTheResultShouldBeReturnedWithTheCorrectCount(int expectedCount, string method,
            string data) => Assert.Equal(expectedCount, _httpHistory.Count(h => h.Method == method && h.Body == data));
    }
}
