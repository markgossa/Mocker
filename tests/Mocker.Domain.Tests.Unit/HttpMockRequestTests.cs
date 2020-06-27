using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpMockRequestTests
    {
        [Fact]
        public void ChecksForInvalidHttpMockRequest()
        {
            var actual = new HttpRequestFilter();

            Assert.IsType<HttpRequestFilter>(actual);
        }

        [Fact]
        public void CreatesHttpMockRequestWithGetProperties()
        {
            var httpQueryFilter = new Dictionary<string, string>
            {
                { "header1", "value" }
            };

            var httpMethodFilter = HttpMethod.Get;
            var actual = new HttpRequestFilter(httpMethodFilter, httpQueryFilter);
            //actual.RouteFilter = "route1";

            //actual.PostBodyFilter = "{\"name\": \"Mark\"}";

            Assert.Equal(httpMethodFilter, actual.Method);
            Assert.Equal(httpQueryFilter, actual.QueryStringFilter);
        }

        [Fact]
        public void CreatesHttpMockRequestWithPostProperties()
        {
            var httpMethodFilter = HttpMethod.Post;
            var httpPostBodyFilter = "{\"name\": \"Mark\"}";

            var actual = new HttpRequestFilter(httpMethodFilter, httpPostBodyFilter);
            //actual.RouteFilter = "route1";


            Assert.Equal(httpMethodFilter, actual.Method);
            Assert.Equal(httpPostBodyFilter, actual.Body);
        }
    }
}
