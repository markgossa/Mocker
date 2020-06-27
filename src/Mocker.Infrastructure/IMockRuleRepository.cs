using Mocker.Domain;
using System.Collections.Generic;

namespace Mocker.Infrastructure
{
    public interface IMockRuleRepository
    {
        List<MockRule> Mocks { get; }

        void Add(MockRule mock);

        void Remove(MockRule mock);

        List<MockRule> GetAllMocks();
    }
}