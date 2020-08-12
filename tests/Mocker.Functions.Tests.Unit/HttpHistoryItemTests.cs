using Mocker.Functions.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Mocker.Functions.Tests.Unit
{
    public class HttpHistoryItemTests
    {
        [Fact]
        public void CanCreateNewValidHttpHistoryItem()
        {
            var expectedTimestamp = DateTime.Parse("10/07/2020");
            var expectedMethod = "get";
            var expectedRoute = "route1";
            var expectedBody = "hello world";
            var expectedHeaders = new Dictionary<string, List<string>>()
            {
                {"header1", new List<string>(){ "value1" } }
            };

            var expectedQuery = new Dictionary<string, string>()
            {
                { "query1", "value1" }
            };

            var sut = new HttpHistoryItem()
            {
                Method = expectedMethod,
                Timestamp = expectedTimestamp,
                Route = expectedRoute,
                Body = expectedBody,
                Headers = expectedHeaders,
                Query = expectedQuery
            };

            Assert.Equal(expectedTimestamp, sut.Timestamp);
            Assert.Equal(expectedMethod, sut.Method);
            Assert.Equal(expectedRoute, sut.Route);
            Assert.Equal(expectedBody, sut.Body);
            Assert.Equal(expectedHeaders, sut.Headers);
            Assert.Equal(expectedQuery, sut.Query);
        }
    }
}
