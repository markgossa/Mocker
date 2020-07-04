using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Mocker.Application.Services
{
    public class HttpMockEngine : IHttpMockEngine
    {
        private readonly IHttpRuleRepository _ruleRepository;

        public HttpMockEngine(IHttpRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public HttpAction Process(HttpRequestDetails httpRequestDetails) =>
            FindFirstMatchingRule(httpRequestDetails)
                ?? BuildDefaultHttpAction();

        private HttpAction? FindFirstMatchingRule(HttpRequestDetails httpRequestDetails)
        {
            var matchingRules = FindAllMatchingRulesIgnoringHeaders(httpRequestDetails);

            if (httpRequestDetails.Headers.Any())
            {
                return FindMatchingRuleThatIgnoresOrMatchesHeaders(httpRequestDetails, matchingRules);
            }

            return matchingRules?.FirstOrDefault()?.HttpAction;
        }

        private HttpAction BuildDefaultHttpAction() => new HttpAction(HttpStatusCode.OK, string.Empty);

        private IEnumerable<HttpRule> FindAllMatchingRulesIgnoringHeaders(HttpRequestDetails httpRequestDetails) =>
            _ruleRepository.Find(httpRequestDetails.Method, httpRequestDetails.Query, httpRequestDetails.Body, httpRequestDetails.Route);

        private HttpAction FindMatchingRuleThatIgnoresOrMatchesHeaders(HttpRequestDetails httpRequestDetails, IEnumerable<HttpRule> matchingRules) => matchingRules
            .Where(r => HttpFilterIgnoresHeaders(r) || HttpRequestDetailsContainsRuleHeaders(r, httpRequestDetails))
            .FirstOrDefault()?.HttpAction ?? BuildDefaultHttpAction();

        private bool HttpFilterIgnoresHeaders(HttpRule httpRule) => httpRule.HttpFilter.IgnoreHeaders;

        private bool HttpRequestDetailsContainsRuleHeaders(HttpRule httpRule, HttpRequestDetails httpRequestDetails) =>
                            httpRule.HttpFilter.Headers?.Count >= 0
                            && httpRule.HttpFilter.Headers.Intersect(httpRequestDetails.Headers).Any();
    }
}
