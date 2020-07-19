using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public class HttpRuleRequestProcessor : IHttpRuleRequestProcessor
    {
        private readonly IHttpRuleRepository _httpRuleRepository;

        public HttpRuleRequestProcessor(IHttpRuleRepository httpRuleRepository)
        {
            _httpRuleRepository = httpRuleRepository;
        }

        public async Task AddAsync(HttpRuleRequest httpRuleRequest)
        {
            var httpFilter = new HttpFilter(new HttpMethod(httpRuleRequest.Filter.Method), httpRuleRequest.Filter.Body,
                httpRuleRequest.Filter.Route, httpRuleRequest.Filter.Query, httpRuleRequest.Filter.Headers, 
                httpRuleRequest.Filter.IgnoreHeaders ?? false);

            var httpAction = new HttpAction(httpRuleRequest.Action.StatusCode, httpRuleRequest.Action.Body,
                httpRuleRequest.Action.Headers, httpRuleRequest.Action.Delay);

            await _httpRuleRepository.AddAsync(new HttpRule(httpFilter, httpAction));
        }
    }
}
