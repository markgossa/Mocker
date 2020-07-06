using Mocker.Application.Contracts;
using Mocker.Application.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public class HistoryRequestProcessor
    {
        private readonly string[] _httpMethods = new string[] {"get", "post", "put", "delete", "head",
            "options", "patch", "trace" };
        private readonly IHttpMockHistoryService _httpMockHistoryService;

        public HistoryRequestProcessor(IHttpMockHistoryService httpMockHistoryService)
        {
            _httpMockHistoryService = httpMockHistoryService;
        }

        public async Task<HttpResponseMessage> ProcessAsync(Dictionary<string, string> query)
        {
            if (!ValidateQuery(query, out var method))
            { 
                return BuildBadRequestMessage();
            }

            await _httpMockHistoryService.FindAsync(new HttpMockHistoryFilter(method, query.GetValueOrDefault("route"), 
                query.GetValueOrDefault("body")));

            return new HttpResponseMessage();
        }

        private bool ValidateQuery(Dictionary<string, string> query, [NotNullWhen(true)] out HttpMethod? method)
        {
            var queryContainsMethod = query.TryGetValue("method", out var value);
            var result = queryContainsMethod && _httpMethods.Any(m => m == value);

            method = result && value != null ? new HttpMethod(value) : null;

            return result;
        }

        private HttpResponseMessage BuildBadRequestMessage() => new HttpResponseMessage()
        {
            Content = new StringContent("Please pass a valid HTTP method to search for"),
            StatusCode = HttpStatusCode.BadRequest
        };


    }
}
