using Mocker.Domain;

namespace Mocker.Application
{
    public interface IHttpMockEngine
    {
        HttpMockResponse Process(HttpRequestDetails httpRequestDetails);
    }
}