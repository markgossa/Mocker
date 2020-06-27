using Mocker.Domain;

namespace Mocker.Application
{
    public interface IHttpMockEngine
    {
        HttpMockAction Process(HttpRequestDetails httpRequestDetails);
    }
}