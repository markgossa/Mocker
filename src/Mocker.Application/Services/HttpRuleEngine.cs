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
        private readonly IHttpRuleRepository _httpRuleRepository;

        public HttpRuleEngine(IHttpRuleRepository httpRuleRepository)
        {
            _httpRuleRepository = httpRuleRepository;
        }

        public async Task<HttpAction> Process(HttpRequestDetails httpRequestDetails) =>
            await FindFirstMatchingRule(httpRequestDetails);

        private async Task<HttpAction> FindFirstMatchingRule(HttpRequestDetails httpRequestDetails)
        {
            var matchingRules = await FindAllMatchingRulesIgnoringHeaders(httpRequestDetails);
            if (matchingRules is null)
            {
                return BuildDefaultHttpAction();
            }

            if (httpRequestDetails.Headers.Any() && !matchingRules.FirstOrDefault().HttpFilter.IgnoreHeaders)
            {
                return FilterMatchingRulesBasedOnHeaders(httpRequestDetails, matchingRules);
            }

            return matchingRules.FirstOrDefault().HttpAction;
        }

        private async Task<IEnumerable<HttpRule>> FindAllMatchingRulesIgnoringHeaders(HttpRequestDetails httpRequestDetails) =>
            await _httpRuleRepository.FindAsync(httpRequestDetails.Method, httpRequestDetails.Body, httpRequestDetails.Route);

        private HttpAction BuildDefaultHttpAction() => new HttpAction(HttpStatusCode.OK, string.Empty);

        private HttpAction FilterMatchingRulesBasedOnHeaders(HttpRequestDetails httpRequestDetails, IEnumerable<HttpRule> matchingRules) => matchingRules
            .Where(r => HttpRequestDetailsContainsRuleHeaders(r, httpRequestDetails))
            .FirstOrDefault()?.HttpAction ?? BuildDefaultHttpAction();

        private bool HttpRequestDetailsContainsRuleHeaders(HttpRule httpRule, HttpRequestDetails httpRequestDetails) =>
                            httpRule.HttpFilter.Headers?.Count >= 0
                            && httpRule.HttpFilter.Headers.Intersect(httpRequestDetails.Headers).Any();
    }
}
