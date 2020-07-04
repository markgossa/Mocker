using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Domain
{
    public class HttpFilter
    {
        public string? Body { get; }
        public Dictionary<string, List<string>>? Headers { get; }
        public bool IgnoreHeaders { get; }
        public HttpMethod? Method { get; }
        public Dictionary<string, string>? Query { get; }
        public string? Route { get; }

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
            Dictionary<string, List<string>>? headers) : this(httpMethod, body, route, query, headers, headers is null)
        {
        }

        public HttpFilter(HttpMethod? httpMethod, string? body, string? route, Dictionary<string, string>? query,
            Dictionary<string, List<string>>? headers, bool ignoreHeaders)
        {
            Method = httpMethod;
            Body = body;
            Route = route;
            Query = query;
            Headers = headers;
            IgnoreHeaders = ignoreHeaders;
        }
    }
}
