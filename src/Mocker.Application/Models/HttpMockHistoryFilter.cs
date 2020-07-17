using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Application.Models
{
    public class HttpMockHistoryFilter
    {
        public string? Body { get; }
        public Dictionary<string, List<string>>? Headers { get; }
        public HttpMethod Method { get; }
        public string? Route { get; }
        public TimeSpan TimeFrame { get; }

        public HttpMockHistoryFilter(HttpMethod method, string? route, string? body, 
            TimeSpan? timeFrame = null, Dictionary<string, List<string>>? headers = null)
        {
            Body = body;
            Headers = headers;
            Method = method;
            Route = route;
            TimeFrame = timeFrame ?? TimeSpan.FromMinutes(5);
        }
    }
}
