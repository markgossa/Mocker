using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Net.Http;

namespace Mocker.Application.Contracts
{
    public interface IHttpRuleRepository
    {
        List<HttpRule> Mocks { get; }

        void Add(HttpRule mock);

        void Remove(HttpRule mock);

        IEnumerable<HttpRule> GetAll();

        IEnumerable<HttpRule> Find(HttpMethod httpMethod, Dictionary<string, string>? queryString,
            string? body, string? route);
    }
}