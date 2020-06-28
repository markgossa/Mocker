using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Mocker.Functions.Models
{
    public class HttpRequestObject
    {
        public Stream BodyStream { get; }
        public IHeaderDictionary Headers { get; }
        public HttpMethod Method { get; }
        public Dictionary<string, string> Query { get; }
        public string? Route { get; }

        public HttpRequestObject(Stream bodyStream, HttpMethod httpMethod,
            Dictionary<string, string> query, IHeaderDictionary headers, string? route = null)
        {
            BodyStream = bodyStream;
            Method = httpMethod;
            Query = query;
            Route = route;
            Headers = headers;
        }
    }
}
