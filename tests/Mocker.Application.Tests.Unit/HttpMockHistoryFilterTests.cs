using Mocker.Application.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Mocker.Application.Tests.Unit
{
    public class HttpMockHistoryFilterTests
    {
        [Fact]
        public void CreatesHttpMockHistoryFilter()
        {
            var method = HttpMethod.Get;
            const string route = "route1";
            const string body = "body1";
            var timeFrame = TimeSpan.FromSeconds(30);
            var headers = new Dictionary<string, List<string>>()
            {
                { "header1", new List<string>(){ "value" } }
            };

            var actual = new HttpMockHistoryFilter(method, route, body, timeFrame, headers);

            Assert.Equal(method, actual.Method);
            Assert.Equal(route, actual.Route);
            Assert.Equal(body, actual.Body);
            Assert.Equal(timeFrame, actual.TimeFrame);
            Assert.Equal(headers, actual.Headers);
        }

        [Fact]
        public void CreatesHttpMockHistoryFilterWithoutTimeSpanReturnsDefaultTimeSpan()
        {
            var method = HttpMethod.Get;
            const string route = "route1";
            const string body = "body1";

            var actual = new HttpMockHistoryFilter(method, route, body);

            Assert.Equal(TimeSpan.FromMinutes(5), actual.TimeFrame);
        }
    }
}
