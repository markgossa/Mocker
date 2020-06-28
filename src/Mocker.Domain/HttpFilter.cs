using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Domain
{
    public class HttpFilter
    {
        public string? Body { get; }
        public Dictionary<string, List<string>>? Headers { get; }
        public HttpMethod? Method { get; }
        public Dictionary<string, string>? Query { get; }
        public string? Route { get; }

        public HttpFilter()
        {
        }

        public HttpFilter(HttpMethod? httpMethod, Dictionary<string, string>? query) : this(httpMethod, null, null, query)
        {
        }

        public HttpFilter(HttpMethod? httpMethod, string? body) : this(httpMethod, body, null, null)
        {
        }

        public HttpFilter(HttpMethod? httpMethod, string? body, string? route, Dictionary<string, string>? query)
            : this(httpMethod, body, route, query, null)
        {
        }

        public HttpFilter(HttpMethod? httpMethod, string? body, string? route, Dictionary<string, string>? query,
            Dictionary<string, List<string>>? headers)
        {
            Method = httpMethod;
            Body = body;
            Route = route;
            Query = query;
            Headers = headers;
        }
    }
}