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
    public class MockerAdminSteps
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _mockerAdminHttpHistoryUrl;
        private readonly Uri _mockerBaseUrl;
        private List<HttpHistoryItem> _httpHistory;

        public MockerAdminSteps(AppSettingsHelper settings)
        {
            _httpClient = new HttpClient();
            _mockerBaseUrl = new Uri(settings.MockerBaseUrl);
            _mockerAdminHttpHistoryUrl = new Uri($"{settings.MockerBaseUrl}/mockeradmin/http/history");
        }

        [Given(@"There is no HTTP history")]
        public async Task ThereisnoHTTPhistory() => await _httpClient.DeleteAsync(_mockerAdminHttpHistoryUrl);

        [Given(@"I have called the HTTP mock using a GET HTTP method")]
        public async Task GivenIHaveCalledTheHTTPMockUsingAGETHTTPMethodAsync() => await _httpClient.GetAsync(_mockerBaseUrl);

        [When(@"I query for that request by HTTP method")]
        public async Task WhenIQueryForThatRequestByHTTPMethod()
        {
            var uri = new UriBuilder(_mockerAdminHttpHistoryUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["method"] = "get";
            uri.Query = query.ToString();

            var response = await _httpClient.GetAsync(uri.ToString());
            var json = await response.Content.ReadAsStringAsync();
            _httpHistory = JsonSerializer.Deserialize<List<HttpHistoryItem>>(json);
        }

        [Then(@"the result should be returned with the correct request count")]
        public void ThenTheResultShouldBeReturnedWithTheCorrectCount() => Assert.Single(_httpHistory.Where(h => h.Method == "GET"));
    }
}
