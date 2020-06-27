using System.Collections.Generic;
using System.Net;

namespace Mocker.Domain
{
    public class HttpMockResponse
    {
        public HttpStatusCode StatusCode { get; }
        public string Body { get; }
        public Dictionary<string, string> Headers { get; }
        public int Delay { get; }

        public HttpMockResponse(HttpStatusCode statusCode, string body) : this(statusCode, body,
            new Dictionary<string, string>())
        {           
        }

        public HttpMockResponse(HttpStatusCode statusCode, string body, Dictionary<string, string> headers)
            : this(statusCode, body, headers, 0)
        {
        }

        public HttpMockResponse(HttpStatusCode statusCode, string body, Dictionary<string, string> headers,
            int delay)
        {
            StatusCode = statusCode;
            Body = body;
            Headers = headers;
            Delay = delay;
        }
    }
}