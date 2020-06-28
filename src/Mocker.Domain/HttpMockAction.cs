using System.Collections.Generic;
using System.Net;

namespace Mocker.Domain
{
    public class HttpMockAction : MockAction
    {
        public HttpStatusCode StatusCode { get; }
        public Dictionary<string, IEnumerable<string>> Headers { get; }

        public HttpMockAction(HttpStatusCode statusCode, string body) : this(statusCode, body,
            new Dictionary<string, IEnumerable<string>>())
        {           
        }

        public HttpMockAction(HttpStatusCode statusCode, string body, Dictionary<string, IEnumerable<string>> headers)
            : this(statusCode, body, headers, 0)
        {
        }

        public HttpMockAction(HttpStatusCode statusCode, string body, Dictionary<string, IEnumerable<string>> headers,
            int delay) : base(body, delay)
        {
            StatusCode = statusCode;
            Headers = headers;
        }
    }
}