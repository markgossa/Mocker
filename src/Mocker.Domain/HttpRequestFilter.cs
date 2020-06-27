using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Domain
{
    public class HttpRequestFilter
    {
        public HttpMethod? Method { get; }
        public Dictionary<string, string>? QueryStringFilter { get; }
        public string? Body { get; }
        public string? Route { get; set; }

        public HttpRequestFilter()
        {

        }

        public HttpRequestFilter(HttpMethod httpMethod, Dictionary<string, string> httpQueryFilter)
        {
            Method = httpMethod;
            QueryStringFilter = httpQueryFilter;
        }

        public HttpRequestFilter(HttpMethod httpMethod, string body)
        {
            Method = httpMethod;
            Body = body;
        }
    }
}