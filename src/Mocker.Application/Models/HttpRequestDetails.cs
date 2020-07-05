using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Application.Models
{
    public class HttpRequestDetails
    {
        public string? Body { get; }
        public Dictionary<string, List<string>> Headers { get; }
        public HttpMethod Method { get; }
        public Dictionary<string, string>? Query { get; }
        public string? Route { get; }
        public DateTime Timestamp { get; }

        public HttpRequestDetails(HttpMethod method, string? route, string? body,
            Dictionary<string, List<string>> headers, Dictionary<string, string>? queryString)
        {
            Body = body;
            Headers = headers;
            Method = method;
            Query = queryString;
            Route = route;
            Timestamp = DateTime.UtcNow;
        }
    }
}
