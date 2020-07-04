using Mocker.Application.Contracts;
using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Mocker.Infrastructure.Services
{
    public class InMemoryHttpRuleRepository : IHttpRuleRepository
    {
        public List<HttpRule> Mocks { get; }

        public InMemoryHttpRuleRepository()
        {
            Mocks = new List<HttpRule>();
        }

        public void Add(HttpRule mock) => Mocks.Add(mock);

        public void Remove(HttpRule mock) => Mocks.Remove(mock);

        public IEnumerable<HttpRule> GetAll() => Mocks;

        public IEnumerable<HttpRule> Find(HttpMethod httpMethod, Dictionary<string, string>? query,
            string? body, string? route) =>
            Mocks.Where(m => m.HttpFilter?.Method == httpMethod
                && m.HttpFilter?.Query == query
                && m.HttpFilter?.Body == body
                && m.HttpFilter?.Route == route);
    }
}
