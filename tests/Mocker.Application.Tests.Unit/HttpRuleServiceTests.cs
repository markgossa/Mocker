using Mocker.Application.Contracts;
using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpRuleServiceTests
    {
        private readonly Mock<IHttpRuleRepository> _mockHttpRuleRepository = new Mock<IHttpRuleRepository>();
        private readonly HttpRuleService _sut;

        public HttpRuleServiceTests()
        {
            _sut = new HttpRuleService(_mockHttpRuleRepository.Object);
        }

        [Fact]
        public async Task AddsRule()
        {
            var httpFilter = new HttpFilter(HttpMethod.Get, "Hello world!");
            var httpAction = new HttpAction(HttpStatusCode.OK, "Hi back!");
            var httpRule = new HttpRule(httpFilter, httpAction);

            await _sut.AddAsync(httpRule);

            _mockHttpRuleRepository.Verify(m => m.AddAsync(It.Is<HttpRule>(h => h == httpRule)));
        }

        [Fact]
        public async Task RemovesAllRules()
        {
            await _sut.RemoveAllAsync();

            _mockHttpRuleRepository.Verify(m => m.RemoveAllAsync());
        }

        [Fact]
        public async Task GetsAllRules()
        {
            await _sut.GetAllAsync();

            _mockHttpRuleRepository.Verify(m => m.GetAllAsync());
        }
    }
}
