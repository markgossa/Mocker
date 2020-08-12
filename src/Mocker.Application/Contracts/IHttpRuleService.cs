using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mocker.Application.Services
{
    public interface IHttpRuleService
    {
        Task AddAsync(HttpRule httpRule);

        Task RemoveAllAsync();

        Task<IEnumerable<HttpRule>> GetAllAsync();
    }
}