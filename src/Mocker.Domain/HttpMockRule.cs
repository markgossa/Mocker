namespace Mocker.Domain
{
    public class HttpMockRule : MockRule
    {
        public HttpRequestFilter? HttpRequestFilter { get; }
        public HttpMockResponse? HttpMockResponse { get; }

        public HttpMockRule()
        {
        }

        public HttpMockRule(HttpMockResponse httpMockResponse) : this(new HttpRequestFilter(), httpMockResponse)
        {
        }

        public HttpMockRule(HttpRequestFilter httpRequestFilter, HttpMockResponse httpMockResponse)
        {
            HttpRequestFilter = httpRequestFilter;
            HttpMockResponse = httpMockResponse;
        }
    }
}
