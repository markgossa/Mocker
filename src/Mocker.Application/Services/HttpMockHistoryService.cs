using Mocker.Application.Contracts;
using Mocker.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocker.Application.Services
{
    public class HttpMockHistoryService
    {
        private readonly IHttpMockHistoryRepository _httpRequestDetailsRepository;

        public HttpMockHistoryService(IHttpMockHistoryRepository httpRequestDetailsRepository)
        {
            _httpRequestDetailsRepository = httpRequestDetailsRepository;
        }

        public async Task AddAsync(HttpRequestDetails httpRequestDetails) => await _httpRequestDetailsRepository.AddAsync(httpRequestDetails);

        public async Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter)
        {
            var matchingRequests = await _httpRequestDetailsRepository.FindAsync(httpMockHistoryFilter);

            return matchingRequests.Where(r => r.Timestamp > DateTime.UtcNow.Add(-httpMockHistoryFilter.TimeFrame)).ToList();
        }

        public async Task DeleteAllAsync() => await _httpRequestDetailsRepository.DeleteAllAsync();
    }
}
