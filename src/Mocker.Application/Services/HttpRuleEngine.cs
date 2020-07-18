using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Mocker.Application.Services
{
    public class HttpRuleEngine : IHttpRuleEngine
    {
        private readonly IHttpRuleRepository _ruleRepository;

        public HttpRuleEngine(IHttpRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public async Task<HttpAction> Process(HttpRequestDetails httpRequestDetails) =>
            await FindFirstMatchingRule(httpRequestDetails) ?? BuildDefaultHttpAction();

        private async Task<HttpAction>? FindFirstMatchingRule(HttpRequestDetails httpRequestDetails)
        {
            var matchingRules = await FindAllMatchingRulesIgnoringHeaders(httpRequestDetails);

            if (httpRequestDetails.Headers.Any())
            {
                return FindMatchingRuleThatIgnoresOrMatchesHeaders(httpRequestDetails, matchingRules);
            }

            return matchingRules?.FirstOrDefault()?.HttpAction;
        }

        private HttpAction BuildDefaultHttpAction() => new HttpAction(HttpStatusCode.OK, string.Empty);

        private async Task<IEnumerable<HttpRule>> FindAllMatchingRulesIgnoringHeaders(HttpRequestDetails httpRequestDetails) =>
            await _ruleRepository.FindAsync(httpRequestDetails.Method, httpRequestDetails.Body, httpRequestDetails.Route);

        private HttpAction FindMatchingRuleThatIgnoresOrMatchesHeaders(HttpRequestDetails httpRequestDetails, IEnumerable<HttpRule> matchingRules) => matchingRules
            .Where(r => HttpFilterIgnoresHeaders(r) || HttpRequestDetailsContainsRuleHeaders(r, httpRequestDetails))
            .FirstOrDefault()?.HttpAction ?? BuildDefaultHttpAction();

        private bool HttpFilterIgnoresHeaders(HttpRule httpRule) => httpRule.HttpFilter.IgnoreHeaders;

        private bool HttpRequestDetailsContainsRuleHeaders(HttpRule httpRule, HttpRequestDetails httpRequestDetails) =>
                            httpRule.HttpFilter.Headers?.Count >= 0
                            && httpRule.HttpFilter.Headers.Intersect(httpRequestDetails.Headers).Any();
    }
}
