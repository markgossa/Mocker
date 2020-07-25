using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Application.Contracts
{
    public interface IHttpRuleRepository
    {
        Task AddAsync(HttpRule httpRule);

        Task RemoveAllAsync();

        Task<List<HttpRule>> GetAllAsync();
    }
}