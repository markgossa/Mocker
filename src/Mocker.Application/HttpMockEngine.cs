using Mocker.Domain;
using System.Linq;
using System.Net;

namespace Mocker.Application
{
    public class HttpMockEngine : IHttpMockEngine
    {
        private readonly IMockHttpRuleRepository _ruleRepository;

        public HttpMockEngine(IMockHttpRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public HttpMockAction Process(HttpRequestDetails httpRequestDetails) => 
            FindFirstMatchingRule(httpRequestDetails)
                ?? new HttpMockAction(HttpStatusCode.OK, string.Empty);

        private HttpMockAction? FindFirstMatchingRule(HttpRequestDetails httpRequestDetails) => 
            _ruleRepository.Find(httpRequestDetails.Method, httpRequestDetails.Query,
                httpRequestDetails.Body, httpRequestDetails.Route)?.FirstOrDefault()?.HttpMockResponse;
    }
}
