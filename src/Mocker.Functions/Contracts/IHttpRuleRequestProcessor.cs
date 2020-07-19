using Mocker.Functions.Models;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public interface IHttpRuleRequestProcessor
    {
        Task AddAsync(HttpRuleRequest httpRuleRequest);
    }
}