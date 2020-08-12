using Mocker.Domain.Models.Http;
using System.Threading.Tasks;

namespace Mocker.Application.Contracts
{
    public interface IHttpRuleEngine
    {
        Task<HttpAction> Process(HttpRequestDetails httpRequestDetails);
    }
}