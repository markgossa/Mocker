using Mocker.Application.Contracts;
using Mocker.Application.Models;
using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocker.Application.Services
{
    public class HttpHistoryService : IHttpHistoryService
    {
        private readonly IHttpMockHistoryRepository _httpRequestDetailsRepository;

        public HttpHistoryService(IHttpMockHistoryRepository httpRequestDetailsRepository)
        {
            _httpRequestDetailsRepository = httpRequestDetailsRepository;
        }

        public async Task AddAsync(HttpRequestDetails httpRequestDetails) => await _httpRequestDetailsRepository.AddAsync(httpRequestDetails);

        public async Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter)
        {
            var httpRequestDetails = (await _httpRequestDetailsRepository.FindAsync(httpMockHistoryFilter)).ToList();

            return httpRequestDetails
                .Where(r => IsMatchingBody(httpMockHistoryFilter, r)
                && IsMatchingHeader(httpMockHistoryFilter, r)
                && IsMatchingRoute(httpMockHistoryFilter, r))
                .ToList();
        }

        private bool IsMatchingRoute(HttpMockHistoryFilter httpMockHistoryFilter, HttpRequestDetails r) =>
            httpMockHistoryFilter.Route is null || httpMockHistoryFilter.Route == r.Route;

        private bool IsMatchingHeader(HttpMockHistoryFilter httpMockHistoryFilter, HttpRequestDetails request)
        {
            var sortedRequestHeaders = request.Headers?.OrderBy(x => x.Key).ToList();
            var sortedFilterHeaders = httpMockHistoryFilter.Headers?.OrderBy(x => x.Key).ToList();

            return httpMockHistoryFilter.Headers is null || IsEqualHeader(sortedRequestHeaders, sortedFilterHeaders);
        }

        private bool IsEqualHeader(List<KeyValuePair<string, List<string>>>? header1, List<KeyValuePair<string, List<string>>>? header2)
        {
            if (header1?.Count != header2?.Count)
            {
                return false;
            }

            for (var i = 0; i < header1.Count(); i++)
            {
                if (header1?[i].Key != header2?[i].Key)
                {
                    return false;
                }

                for (var n = 0; n < header1?[i].Value.Count; n++)
                {
                    if (header1[i].Value[n] != header2?[i].Value[n])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsMatchingBody(HttpMockHistoryFilter httpMockHistoryFilter, HttpRequestDetails request) => 
            httpMockHistoryFilter.Body is null || request.Body == httpMockHistoryFilter.Body;

        public async Task DeleteAllAsync() => await _httpRequestDetailsRepository.DeleteAllAsync();
    }
}
