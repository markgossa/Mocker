using Mocker.Application;
using Mocker.Functions.Contracts;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Models
{
    public class HttpRequestProcessor : IHttpRequestProcessor
    {
        private readonly IHttpMockEngine _httpMockEngine;
        private readonly IMapper<HttpRequestObject, Task<HttpRequestDetails>> _mapper;

        public HttpRequestProcessor(IHttpMockEngine httpMockEngine, 
            IMapper<HttpRequestObject, Task<HttpRequestDetails>> mapper)
        {
            _httpMockEngine = httpMockEngine;
            _mapper = mapper;
        }

        public async Task<HttpResponseMessage> ProcessAsync(HttpRequestObject httpRequestObject)
        {
            var httpRequestDetails = await _mapper.Map(httpRequestObject);

            var mockResponse = _httpMockEngine.Process(httpRequestDetails);
            var response = new HttpResponseMessage(mockResponse.StatusCode)
            {
                Content = new StringContent(mockResponse.Body)
            };

            AddHeaders(mockResponse.Headers, response);

            return response;
        }

        private void AddHeaders(Dictionary<string, IEnumerable<string>> newHeaders, HttpResponseMessage response)
        {
            foreach (var header in newHeaders)
            {
                response.Content.Headers.Add(header.Key, string.Join(",", header.Value));
            }
        }
    }
}
