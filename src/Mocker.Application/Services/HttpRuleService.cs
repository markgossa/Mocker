using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocker.Application.Services
{
    public class HttpRuleService : IHttpRuleService
    {
        private readonly IHttpRuleRepository _httpRuleRepository;

        public HttpRuleService(IHttpRuleRepository httpRuleRepository)
        {
            _httpRuleRepository = httpRuleRepository;
        }

        public async Task AddAsync(HttpRule httpRule) => await _httpRuleRepository.AddAsync(httpRule);

        public async Task RemoveAllAsync() => await _httpRuleRepository.RemoveAllAsync();

        public async Task<IEnumerable<HttpRule>> GetAllAsync()
        {
            var cachedRules = await _httpRuleRepository.GetCachedRulesAsync();
            var getRuleDetailsTasks = new List<Task<HttpRule?>>();
            foreach (var cachedRule in cachedRules)
            {
                getRuleDetailsTasks.Add(_httpRuleRepository.GetRuleDetailsAsync(cachedRule.Id));
            }

            var rules = await Task.WhenAll(getRuleDetailsTasks);
            return rules as IEnumerable<HttpRule> ?? new List<HttpRule>();
        }
    }
}
