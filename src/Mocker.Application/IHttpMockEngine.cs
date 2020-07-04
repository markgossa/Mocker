using Mocker.Domain;

namespace Mocker.Application
{
    public interface IHttpMockEngine
    {
        HttpAction Process(HttpRequestDetails httpRequestDetails);
    }
}