namespace Mocker.Domain
{
    public class HttpMockRule : MockRule
    {
        public HttpFilter? HttpRequestFilter { get; }
        public HttpMockAction? HttpMockResponse { get; }

        public HttpMockRule()
        {
        }

        public HttpMockRule(HttpMockAction httpMockResponse) : this(new HttpFilter(), httpMockResponse)
        {
        }

        public HttpMockRule(HttpFilter httpRequestFilter, HttpMockAction httpMockResponse)
        {
            HttpRequestFilter = httpRequestFilter;
            HttpMockResponse = httpMockResponse;
        }
    }
}
