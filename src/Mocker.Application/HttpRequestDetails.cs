using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Application
{
    public class HttpRequestDetails
    {
        public HttpMethod Method { get; }
        public string? Route { get; }
        public string? Body { get; }
        public Dictionary<string, IEnumerable<string>>? Headers { get; }
        public Dictionary<string, string>? Query { get; }

        public HttpRequestDetails(HttpMethod method, string? route, string? body, 
            Dictionary<string, IEnumerable<string>>? headers, Dictionary<string, string>? queryString)
        {
            Method = method;
            Route = route;
            Body = body;
            Headers = headers;
            Query = queryString;
        }
    }
}
