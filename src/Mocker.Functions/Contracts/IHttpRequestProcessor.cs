using Mocker.Functions.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Contracts
{
    public interface IHttpRequestProcessor
    {
        Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestObject httpRequestObject);
    }
}