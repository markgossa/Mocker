using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Domain
{
    public class HttpFilter
    {
        public HttpMethod? Method { get; }
        public IDictionary<string, string>? QueryString { get; }
        public string? Body { get; }
        public string? Route { get; }

        public HttpFilter()
        {
        }

        public HttpFilter(HttpMethod? httpMethod, Dictionary<string, string>? queryString)
        {
            Method = httpMethod;
            QueryString = queryString;
        }

        public HttpFilter(HttpMethod? httpMethod, string? body) : this(httpMethod, body, null, null)
        {
        }

        public HttpFilter(HttpMethod? httpMethod, string? body, string? route, IDictionary<string, string>? queryString)
        {
            Method = httpMethod;
            Body = body;
            Route = route;
            QueryString = queryString;
        }
    }
}