﻿using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Contracts;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public class HttpRequestProcessor : IHttpRequestProcessor
    {
        private readonly IHttpRuleEngine _httpMockEngine;
        private readonly IHttpHistoryService _httpMockHistoryService;
        private readonly IMapper<HttpRequestObject, Task<HttpRequestDetails>> _mapper;

        public HttpRequestProcessor(IHttpRuleEngine httpMockEngine, IHttpHistoryService httpMockHistoryService,
            IMapper<HttpRequestObject, Task<HttpRequestDetails>> mapper)
        {
            _httpMockEngine = httpMockEngine;
            _httpMockHistoryService = httpMockHistoryService;
            _mapper = mapper;
        }

        public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestObject httpRequestObject)
        {
            var httpRequestDetails = await _mapper.Map(httpRequestObject);
            var loggingTask = _httpMockHistoryService.AddAsync(httpRequestDetails);

            var httpAction = await _httpMockEngine.Process(httpRequestDetails);
            var response = new HttpResponseMessage(httpAction.StatusCode)
            {
                Content = new StringContent(httpAction.Body)
            };

            AddHeaders(httpAction.Headers, response);

            await loggingTask;

            return response;
        }

        private void AddHeaders(Dictionary<string, List<string>>? headersToAdd, HttpResponseMessage response)
        {
            if (headersToAdd is null)
            {
                return;
            }

            foreach (var header in headersToAdd)
            {
                response.Content.Headers.Add(header.Key, string.Join(",", header.Value));
            }
        }
    }
}
