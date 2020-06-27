using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Application
{
    public class HttpRequestDetails
    {
        public HttpMethod? Method { get; }
        public string? Route { get; }
        public string? Body { get; }
        public Dictionary<string, string>? Headers { get; }
        public string? QueryString { get; }

        public HttpRequestDetails(HttpMethod method, string route, string body, 
            Dictionary<string, string> headers, string queryString)
        {
            Method = method;
            Route = route;
            Body = body;
            Headers = headers;
            QueryString = queryString;
        }
    }
}
