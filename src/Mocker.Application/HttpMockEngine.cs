using Mocker.Domain;
using Mocker.Infrastructure;
using System.Linq;
using System.Net;

namespace Mocker.Application
{
    public class HttpMockEngine : IHttpMockEngine
    {
        private readonly IMockRuleRepository _mockRuleRepository;

        public HttpMockEngine(IMockRuleRepository mockRuleRepository)
        {
            _mockRuleRepository = mockRuleRepository;
        }

        public HttpMockResponse Process(HttpRequestDetails httpRequestDetails) => _mockRuleRepository.GetAllMocks()?
                .Select(r => r as HttpMockRule)?
                .FirstOrDefault(r => r?.HttpRequestFilter?.Body == httpRequestDetails.Body
                    && r?.HttpRequestFilter?.Method == httpRequestDetails.Method
                    && r?.HttpRequestFilter?.Route == httpRequestDetails.Route)?
                .HttpMockResponse ?? new HttpMockResponse(HttpStatusCode.OK, string.Empty);
    }
}
