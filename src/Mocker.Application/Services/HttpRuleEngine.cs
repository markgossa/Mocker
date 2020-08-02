using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using Mocker.Application.Models;
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

        public async Task<HttpAction> Process(HttpRequestDetails httpRequestDetails)
        {
            var firstMathingHttpRule = (await _httpRuleRepository.GetCachedRulesAsync())
                .Where(r => IsNullOrMatchingMethod(r, httpRequestDetails)
                && IsNullOrMatchingBody(r, httpRequestDetails)
                && IsNullOrMatchingRoute(r, httpRequestDetails)
                && IsNullOrMatchingHeader(r, httpRequestDetails)
                && IsNullOrMatchingQuery(r, httpRequestDetails)
                ).FirstOrDefault();

            var httpAction = (await _httpRuleRepository.GetRuleDetailsAsync(firstMathingHttpRule?.Id ?? 0))?
                .HttpAction ?? BuildDefaultHttpAction();

            await ApplyHttpActionDelay(httpAction);

            return httpAction;
        }

        private bool IsNullOrMatchingMethod(HttpRule rule, HttpRequestDetails httpRequestDetails) => 
            rule.HttpFilter.Method == null || rule.HttpFilter.Method == httpRequestDetails.Method;
        
        private bool IsNullOrMatchingBody(HttpRule rule, HttpRequestDetails httpRequestDetails) => 
            rule.HttpFilter.Body == null || rule.HttpFilter.Body == httpRequestDetails.Body;

        private bool IsNullOrMatchingRoute(HttpRule rule, HttpRequestDetails httpRequestDetails) =>
            rule.HttpFilter.Route == null || rule.HttpFilter.Route == httpRequestDetails.Route;

        private bool IsNullOrMatchingHeader(HttpRule rule, HttpRequestDetails httpRequestDetails) =>
            rule.HttpFilter.Headers == null || httpRequestDetails.Headers.Contains(rule.HttpFilter.Headers);

        private async Task ApplyHttpActionDelay(HttpAction httpAction)
        {
            if (httpAction.Delay > 0)
            {
                await Task.Delay(httpAction.Delay);
            }
        }

        private bool IsNullOrMatchingQuery(HttpRule rule, HttpRequestDetails httpRequestDetails) => rule.HttpFilter.Query == null
                || (httpRequestDetails.Query != null
                && httpRequestDetails.Query.IsEqualTo(rule.HttpFilter.Query));

        private HttpAction BuildDefaultHttpAction() => new HttpAction(HttpStatusCode.OK, string.Empty);
    }
}
