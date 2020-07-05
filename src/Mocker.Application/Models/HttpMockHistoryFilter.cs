using System;
using System.Net.Http;

namespace Mocker.Application.Models
{
    public class HttpMockHistoryFilter
    {
        public HttpMethod Method { get; }
        public string? Route { get; }
        public string? Body { get; }
        public TimeSpan TimeFrame { get; }

        public HttpMockHistoryFilter(HttpMethod method, string? route, string? body)
            : this(method, route, body, null)
        {
        }
        
        public HttpMockHistoryFilter(HttpMethod method, string? route, string? body, TimeSpan? timeFrame)
        {
            Method = method;
            Route = route;
            Body = body;
            TimeFrame = timeFrame ?? TimeSpan.FromMinutes(5);
        }
    }
}
