using Mocker.Domain;
using Mocker.Infrastructure;
using System.Linq;
using System.Net;

namespace Mocker.Application
{
    public class HttpMockEngine : IHttpMockEngine
    {
        private readonly IMockHttpRuleRepository _ruleRepository;

        public HttpMockEngine(IMockHttpRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public HttpMockAction Process(HttpRequestDetails httpRequestDetails) => _ruleRepository.GetAll()?
                .FirstOrDefault(r => r?.HttpRequestFilter?.Body == httpRequestDetails.Body
                    && r?.HttpRequestFilter?.Method == httpRequestDetails.Method
                    && r?.HttpRequestFilter?.Route == httpRequestDetails.Route)?
                .HttpMockResponse ?? new HttpMockAction(HttpStatusCode.OK, string.Empty);
    }
}
