using Mocker.Application;
using Mocker.Functions.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace Mocker.Functions.Models
{
    public class HttpRequestDetailsMapper : IMapper<HttpRequestObject, Task<HttpRequestDetails>>
    {
        public async Task<HttpRequestDetails> Map(HttpRequestObject httpRequest)
        {
            using var streamReader = new StreamReader(httpRequest.BodyStream);
            var body = await streamReader.ReadToEndAsync();
                
            return new HttpRequestDetails(httpRequest.Method, httpRequest.Route, body, null, httpRequest.Query);
        }
    }
}
