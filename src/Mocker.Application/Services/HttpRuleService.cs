using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using System.Collections.Generic;
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

        public async Task<IEnumerable<HttpRule>> GetAllAsync() => await _httpRuleRepository.GetAllAsync();
    }
}
