using Mocker.Application.Contracts;
using Mocker.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Infrastructure.Services
{
    public class InMemoryHttpMockHistoryRepository : IHttpMockHistoryRepository
    {
        public List<HttpRequestDetails> Requests { get; }

        public InMemoryHttpMockHistoryRepository()
        {
            Requests = new List<HttpRequestDetails>();
        }

        public async Task AddAsync(HttpRequestDetails httpRequestDetails) => Requests.Add(httpRequestDetails);

        public async Task DeleteAllAsync() => Requests.Clear();
        public async Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter) => Requests
            .Where(r => 
                r.Method == httpMockHistoryFilter.Method
                && r.Route == httpMockHistoryFilter.Route
                && r.Timestamp > DateTime.UtcNow.Add(-httpMockHistoryFilter.TimeFrame))
            .ToList();

        public async Task<List<HttpRequestDetails>> FindByMethodAsync(HttpMethod httpMethod) => Requests
            .Where(r =>
                r.Method == httpMethod)
            .ToList();
    }
}
