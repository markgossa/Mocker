using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Domain.Models.Http
{
    public class HttpFilter
    {
        public string? Body { get; }
        public Dictionary<string, List<string>>? Headers { get; }
        public HttpMethod? Method { get; }
        public Dictionary<string, string>? Query { get; }
        public string? Route { get; }

        public HttpFilter(HttpMethod? httpMethod, string? body = null, string? route = null, 
            Dictionary<string, string>? query = null, Dictionary<string, List<string>>? headers = null)
        {
            Method = httpMethod;
            Body = body;
            Route = route;
            Query = query;
            Headers = headers;
        }
    }
}
