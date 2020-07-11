using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public class HistoryQueryProcessor : IHistoryQueryProcessor
    {
        private readonly string[] _httpMethods = new string[] {"get", "post", "put", "delete", "head",
            "options", "patch", "trace" };
        private readonly IHttpHistoryService _httpMockHistoryService;

        public HistoryQueryProcessor(IHttpHistoryService httpMockHistoryService)
        {
            _httpMockHistoryService = httpMockHistoryService;
        }

        public async Task<HttpResponseMessage> ProcessAsync(Dictionary<string, string> query)
        {
            if (!ValidateMethodQuery(query, out var method) || method is null)
            {
                return BuildBadRequestMessage("Please pass a valid HTTP method to search for");
            }

            if (!ValidateTimeframeQuery(query, out var timeframe))
            {
                return BuildBadRequestMessage("Please pass a valid timeframe to search for");
            }

            var httpHistory = await _httpMockHistoryService.FindAsync(new HttpMockHistoryFilter(method, query.GetValueOrDefault("route"),
                query.GetValueOrDefault("body"), timeframe));
            var httpHistoryItems = MapToHttpHistoryItems(httpHistory);
            
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(httpHistoryItems))
            };
        }

        private IEnumerable<HttpHistoryItem>? MapToHttpHistoryItems(List<HttpRequestDetails> httpHistory)
            => httpHistory?.Select(h => new HttpHistoryItem()
            {
                Body = h.Body,
                Headers = h.Headers,
                Method = h.Method.ToString(),
                Query = h.Query,
                Route = h.Route,
                Timestamp = h.Timestamp
            });

        private bool ValidateMethodQuery(Dictionary<string, string> query, out HttpMethod? method)
        {
            var queryContainsMethod = query.TryGetValue("method", out var methodValue);
            var result = !string.IsNullOrWhiteSpace(methodValue) && queryContainsMethod && _httpMethods.Any(m => m == methodValue);

            method = result ? new HttpMethod(methodValue) : null;

            return result;
        }

        private bool ValidateTimeframeQuery(Dictionary<string, string> query, out TimeSpan? timeframe)
        {
            timeframe = null;
            if (query.TryGetValue("timeframe", out var timeframeString))
            {
                if (TimeSpan.TryParse(timeframeString, out var timeframeValue))
                {
                    timeframe = timeframeValue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private HttpResponseMessage BuildBadRequestMessage(string content) => new HttpResponseMessage()
        {
            Content = new StringContent(content),
            StatusCode = HttpStatusCode.BadRequest
        };
    }
}
