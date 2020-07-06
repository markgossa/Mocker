using System.Net.Http;

namespace Mocker.Functions.Models
{
    public class HttpMockHistoryFilterRequest
    {
        public HttpMethod Method { get; set; }
        public string? Route { get; set; }
        public string? Body { get; set; }

        public HttpMockHistoryFilterRequest(HttpMethod httpMethod, string? route = null,
            string? body = null)
        {
            Method = httpMethod;
            Route = route;
            Body = body;
        }
    }
}
