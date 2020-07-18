using System.Collections.Generic;
using System.Net;

namespace Mocker.Domain.Models.Http
{
    public class HttpAction : MockAction
    {
        public HttpStatusCode StatusCode { get; }
        public Dictionary<string, List<string>>? Headers { get; }

        public HttpAction(HttpStatusCode statusCode, string? body) : this(statusCode, body,
            new Dictionary<string, List<string>>())
        {
        }

        public HttpAction(HttpStatusCode statusCode, string? body, Dictionary<string, List<string>>? headers)
            : this(statusCode, body, headers, 0)
        {
        }

        public HttpAction(HttpStatusCode statusCode, string? body, Dictionary<string, List<string>>? headers,
            int delay) : base(body, delay)
        {
            StatusCode = statusCode;
            Headers = headers;
        }
    }
}