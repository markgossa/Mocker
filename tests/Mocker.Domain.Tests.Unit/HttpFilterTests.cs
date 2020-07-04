using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Mocker.Domain.Tests.Unit
{
    public class HttpFilterTests
    {
        [Fact]
        public void CreatesHttpFilterWithGetProperties()
        {
            var httpQueryFilter = new Dictionary<string, string>
            {
                { "header1", "value" }
            };

            var httpMethodFilter = HttpMethod.Get;
            var actual = new HttpFilter(httpMethodFilter, httpQueryFilter);

            Assert.Equal(httpMethodFilter, actual.Method);
            Assert.Equal(httpQueryFilter, actual.Query);
        }

        [Fact]
        public void CreatesHttpFilterWithPostProperties()
        {
            var httpMethodFilter = HttpMethod.Post;
            var httpPostBodyFilter = "{\"name\": \"Mark\"}";

            var actual = new HttpFilter(httpMethodFilter, httpPostBodyFilter);

            Assert.Equal(httpMethodFilter, actual.Method);
            Assert.Equal(httpPostBodyFilter, actual.Body);
        }

        [Fact]
        public void CreatesHttpFilterWithIgnoreHeadersTrueIfHeadersNotSpecified()
        {
            var httpMethodFilter = HttpMethod.Post;
            var httpPostBodyFilter = "{\"name\": \"Mark\"}";

            var actual = new HttpFilter(httpMethodFilter, httpPostBodyFilter);

            Assert.Equal(httpMethodFilter, actual.Method);
            Assert.Equal(httpPostBodyFilter, actual.Body);
            Assert.True(actual.IgnoreHeaders);
        }

        [Fact]
        public void CreatesHttpFilterWithMatchHeadersTrueIfHeadersSpecified()
        {
            var httpMethodFilter = HttpMethod.Post;
            
            var actual = new HttpFilter(httpMethodFilter, null, null,
                null, new Dictionary<string, List<string>>());

            Assert.Equal(httpMethodFilter, actual.Method);
            Assert.False(actual.IgnoreHeaders);
        }

        [Fact]
        public void CanCreatesHttpFilterWithMatchHeadersTrueIfHeadersNotSpecified()
        {
            var httpMethodFilter = HttpMethod.Post;

            var actual = new HttpFilter(httpMethodFilter, null, null,
                null, null, false);

            Assert.Equal(httpMethodFilter, actual.Method);
            Assert.False(actual.IgnoreHeaders);
        }
    }
}
