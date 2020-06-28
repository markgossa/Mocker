using Mocker.Domain;
using System.Collections.Generic;
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

        private HttpMockAction? FindFirstMatchingRule(HttpRequestDetails httpRequestDetails)
        {
            var matchingRules = _ruleRepository.Find(httpRequestDetails.Method, httpRequestDetails.Query,
                httpRequestDetails.Body, httpRequestDetails.Route);

            if (httpRequestDetails.Headers.Any())
            {
                var firstHeaderToMatch = httpRequestDetails.Headers.FirstOrDefault();
                return matchingRules?.Where(r => httpRequestDetails.Headers.ContainsKey(r.HttpRequestFilter.Headers.FirstOrDefault().Key))
                    .FirstOrDefault().HttpMockResponse;
            }

            return matchingRules?.FirstOrDefault()?.HttpMockResponse;
        }
    }
}
