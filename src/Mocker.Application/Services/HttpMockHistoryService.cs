using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocker.Application.Services
{
    public class HttpMockHistoryService : IHttpMockHistoryService
    {
        private readonly IHttpMockHistoryRepository _httpRequestDetailsRepository;

        public HttpMockHistoryService(IHttpMockHistoryRepository httpRequestDetailsRepository)
        {
            _httpRequestDetailsRepository = httpRequestDetailsRepository;
        }

        public async Task AddAsync(HttpRequestDetails httpRequestDetails) => await _httpRequestDetailsRepository.AddAsync(httpRequestDetails);

        public async Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter)
        {
            List<HttpRequestDetails> matchingRequests;
            if (httpMockHistoryFilter.Body is null && httpMockHistoryFilter.Route is null)
            {
                matchingRequests = await _httpRequestDetailsRepository.FindByMethodAsync(httpMockHistoryFilter.Method);
            }
            else
            {
                matchingRequests = await _httpRequestDetailsRepository.FindAsync(httpMockHistoryFilter);
            }

            return matchingRequests.Where(IsHttpRequestWithinTimeFrame(httpMockHistoryFilter)).ToList();
        }

        private static Func<HttpRequestDetails, bool> IsHttpRequestWithinTimeFrame(HttpMockHistoryFilter httpMockHistoryFilter) => r => 
            r.Timestamp > DateTime.UtcNow.Add(-httpMockHistoryFilter.TimeFrame);

        public async Task DeleteAllAsync() => await _httpRequestDetailsRepository.DeleteAllAsync();
    }
}
