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
    public class HttpHistoryQueryService : IHttpHistoryQueryService
    {
        private readonly string[] _httpMethods = new string[] {"get", "post", "put", "delete", "head",
            "options", "patch", "trace" };
        private readonly IHttpHistoryService _httpMockHistoryService;

        public HttpHistoryQueryService(IHttpHistoryService httpMockHistoryService)
        {
            _httpMockHistoryService = httpMockHistoryService;
        }

        public async Task<HttpResponseMessage> ExecuteQueryAsync(Dictionary<string, string> query)
        {
            if (!ValidateMethodQuery(query, out var method) || method is null)
            {
                return BuildBadRequestMessage("Please pass a valid HTTP method to search for");
            }

            if (!ValidateTimeframeQuery(query, out var timeframe))
            {
                return BuildBadRequestMessage("Please pass a valid timeframe to search for");
            }

            if (!ValidateHeaderQuery(query, out var headers))
            {
                return BuildBadRequestMessage("Please pass a valid header in the query string to search " +
                    "for e.g. header=key1=value1,key2=value2");
            }

            var httpHistory = await _httpMockHistoryService.FindAsync(new HttpMockHistoryFilter(method, 
                query.GetValueOrDefault("route"), query.GetValueOrDefault("body"), timeframe, headers));
            var httpHistoryItems = MapToHttpHistoryItems(httpHistory);
            
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(httpHistoryItems))
            };
        }

        private bool ValidateHeaderQuery(Dictionary<string, string> query, out Dictionary<string, List<string>>? headers)
        {
            headers = null;
            if (query.TryGetValue("headers", out var headersString))
            {
                if (string.IsNullOrWhiteSpace(headersString))
                {
                    return false;
                }

                headers = ConvertToHeaders(headersString);

                return !headers.Values.Any(v => v.Any(a => string.IsNullOrWhiteSpace(a)));
            }

            return true;
        }

        private Dictionary<string, List<string>> ConvertToHeaders(string headers)
        {
            var headersDictionary = new Dictionary<string, List<string>>();
            foreach (var header in headers.Split(','))
            {
                var headerParts = header.Split('=');
                if (headerParts.Count() % 2 == 0)
                {
                    headersDictionary.Add(headerParts[0], new List<string>() { headerParts[1] });
                }
            }

            return headersDictionary;
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
            var result = !string.IsNullOrWhiteSpace(methodValue) && queryContainsMethod
                && _httpMethods.Any(m => m.ToLower() == methodValue.ToLower());

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
