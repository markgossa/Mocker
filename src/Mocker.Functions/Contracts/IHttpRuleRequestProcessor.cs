using Mocker.Functions.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public interface IHttpRuleRequestProcessor
    {
        Task AddAsync(HttpRuleRequest httpRuleRequest);
        Task<HttpRuleResponse> GetAllAsync();
        Task RemoveAllAsync();
        bool Validate(HttpRuleRequest httpRuleRequest, out List<ValidationResult> validationResults);
    }
}