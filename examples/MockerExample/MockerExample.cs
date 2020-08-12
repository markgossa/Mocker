using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace TestProject.Tests
{
    public class MockerExample
    {
        private const string _mockerAdminBaseUrl = "http://localhost:7071";

        [Theory]
        [InlineData("Paris", "10")]
        [InlineData("Kuala Lumpur", "31")]
        public async Task TestCorrectWeatherServiceTemperature(string location, string temperature)
        {
            using var httpClient = new HttpClient();

            var expectedTemperature = int.Parse(temperature);

            // ARRANGE
            // Clear all rules from mocker
            await httpClient.DeleteAsync($"{_mockerAdminBaseUrl}/mockeradmin/http/rules");

            // Clear all request history from mocker
            await httpClient.DeleteAsync($"{_mockerAdminBaseUrl}/mockeradmin/http/history");

            // Add new mocker rule which returns the correct temperature based on location
            var expectedBody = JsonSerializer.Serialize(new WeatherServiceTemperatureRequest(location));
            var expectedRoute = "getLocalWeather";
            var newMockerRule = new HttpRuleRequest
            {
                Filter = new HttpRuleFilterRequest
                {
                    Body = expectedBody,
                    Headers = new Dictionary<string, List<string>> { { "code", new List<string> { "12345" } } },
                    Method = "POST",
                    Query = new Dictionary<string, string> { { "temperatureOnly", "true" } },
                    Route = expectedRoute
                },
                Action = new HttpRuleActionRequest
                {
                    Body = JsonSerializer.Serialize(new WeatherServiceTemperatureResponse { Temperature = expectedTemperature }),
                    Delay = 500,
                    Headers = new Dictionary<string, List<string>> { { "result", new List<string> { "success" } } },
                    StatusCode = HttpStatusCode.OK
                }
            };

            var jsonRequest = JsonSerializer.Serialize(newMockerRule);
            using var httpContent = new StringContent(jsonRequest);
            await httpClient.PostAsync($"{_mockerAdminBaseUrl}/mockeradmin/http/rules", httpContent);

            // ACT
            var sut = new WeatherService();
            var actual = await sut.GetTemperature(location);

            // ASSERT
            // Assert correct response from SUT
            Assert.Equal(expectedTemperature, actual);

            // Assert correct request was sent to the Weather API
            var response = await httpClient.GetAsync($"{_mockerAdminBaseUrl}/mockeradmin/http/history?method=POST");
            var json = await response.Content.ReadAsStringAsync();
            var requests = JsonSerializer.Deserialize<List<HttpHistoryItem>>(json);
            Assert.Single(requests);
            Assert.Equal(expectedBody, requests[0].Body);
            Assert.Equal(expectedRoute, requests[0].Route);
        }
    }

    public class WeatherService
    {
        private readonly string _weatherServiceUrl = "http://localhost:7071/getLocalWeather?temperatureOnly=true";

        public async Task<int> GetTemperature(string location)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("code", "12345");

            var jsonRequest = JsonSerializer.Serialize(new WeatherServiceTemperatureRequest(location));
            using var httpContent = new StringContent(jsonRequest);
            var response = await httpClient.PostAsync(_weatherServiceUrl, httpContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<WeatherServiceTemperatureResponse>(jsonResponse);

            return weatherData.Temperature;
        }
    }

    public class WeatherServiceTemperatureRequest
    {
        public string Location { get; }

        public WeatherServiceTemperatureRequest(string location)
        {
            Location = location;
        }
    }

    public class WeatherServiceTemperatureResponse
    {
        public int Temperature { get; set; }
    }

    public class HttpRuleRequest
    {
        public HttpRuleFilterRequest Filter { get; set; }

        public HttpRuleActionRequest Action { get; set; }
    }

    public class HttpRuleFilterRequest
    {
        public string Body { get; set; }
        public Dictionary<string, List<string>> Headers { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public string Route { get; set; }
    }

    public class HttpRuleActionRequest
    {
        public string Body { get; set; }
        public int Delay { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, List<string>> Headers { get; set; }
    }

    public class HttpHistoryItem
    {
        public string Body { get; set; }
        public Dictionary<string, List<string>> Headers { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public string Route { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
