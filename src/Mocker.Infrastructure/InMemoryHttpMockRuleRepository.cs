using Mocker.Application;
using Mocker.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Mocker.Infrastructure
{
    public class InMemoryHttpMockRuleRepository : IMockHttpRuleRepository
    {
        public List<HttpMockRule> Mocks { get; }

        public InMemoryHttpMockRuleRepository()
        {
            Mocks = new List<HttpMockRule>();
        }

        public void Add(HttpMockRule mock) => Mocks.Add(mock);

        public void Remove(HttpMockRule mock) => Mocks.Remove(mock);

        public IEnumerable<HttpMockRule> GetAll() => Mocks;

        public IEnumerable<HttpMockRule> Find(HttpMethod httpMethod, Dictionary<string, string>? query, 
            string? body, string? route) => 
            Mocks.Where(m => m.HttpRequestFilter?.Method == httpMethod
                && m.HttpRequestFilter?.Query == query
                && m.HttpRequestFilter?.Body == body
                && m.HttpRequestFilter?.Route == route);
    }
}
