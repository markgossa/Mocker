using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public class HttpRequestProcessor : IHttpRequestProcessor
    {
        private readonly IHttpMockEngine _httpMockEngine;
        private readonly IHttpMockHistoryService _httpMockHistoryService;
        private readonly IMapper<HttpRequestObject, Task<HttpRequestDetails>> _mapper;

        public HttpRequestProcessor(IHttpMockEngine httpMockEngine, IHttpMockHistoryService httpMockHistoryService,
            IMapper<HttpRequestObject, Task<HttpRequestDetails>> mapper)
        {
            _httpMockEngine = httpMockEngine;
            _httpMockHistoryService = httpMockHistoryService;
            _mapper = mapper;
        }

        public async Task<HttpResponseMessage> ProcessAsync(HttpRequestObject httpRequestObject)
        {
            var httpRequestDetails = await _mapper.Map(httpRequestObject);

            var loggingTask = _httpMockHistoryService.AddAsync(httpRequestDetails);

            var httpAction = _httpMockEngine.Process(httpRequestDetails);
            var response = new HttpResponseMessage(httpAction.StatusCode)
            {
                Content = new StringContent(httpAction.Body)
            };

            AddHeaders(httpAction.Headers, response);

            await loggingTask;

            return response;
        }

        private void AddHeaders(Dictionary<string, List<string>> headersToAdd, HttpResponseMessage response)
        {
            foreach (var header in headersToAdd)
            {
                response.Content.Headers.Add(header.Key, string.Join(",", header.Value));
            }
        }
    }
}
