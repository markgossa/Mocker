using Mocker.Domain.Models.Http;

namespace Mocker.Application.Contracts
{
    public interface IHttpMockEngine
    {
        HttpAction Process(HttpRequestDetails httpRequestDetails);
    }
}