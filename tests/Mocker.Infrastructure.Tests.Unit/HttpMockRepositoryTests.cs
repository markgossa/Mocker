using Mocker.Domain;
using System.Linq;
using Xunit;

namespace Mocker.Infrastructure.Tests.Unit
{
    public class HttpMockRepositoryTests
    {
        private readonly InMemoryMockRuleRepository _sut;

        public HttpMockRepositoryTests()
        {
            _sut = new InMemoryMockRuleRepository();
        }

        [Fact]
        public void MockHttpRepositorySavesMockHttpResponses()
        {
            var httpMock = new HttpMockRule();
            _sut.Add(httpMock);
            var actual = _sut.GetAllMocks();

            Assert.True(actual.Count(m => m == httpMock) == 1);
        }

        [Fact]
        public void MockHttpRepositoryDeletesMockHttpResponses()
        {
            var httpMock = new HttpMockRule();
            _sut.Add(httpMock);
            _sut.Remove(httpMock);
            var actual = _sut.GetAllMocks();

            Assert.DoesNotContain(actual, m => m == httpMock);
        }

        //[Fact]
        //public void MockHttpRepositoryUpdatesMockHttpResponses()
        //{
        //    var httpMock = new HttpMock();
        //    _sut.Add(httpMock);

        //    var updatedHttpMock = new HttpMock(new HttpMockResponse(HttpStatusCode.OK, string.Empty));
        //    _sut.Update(updatedHttpMock);
        //    var actual = _sut.GetAllMocks();

        //    Assert.DoesNotContain(actual, m => m == httpMock);
        //}
    }
}
