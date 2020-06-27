using Mocker.Domain;
using System.Collections.Generic;

namespace Mocker.Infrastructure
{
    public class InMemoryMockRuleRepository : IMockRuleRepository
    {
        public List<MockRule> Mocks { get; }

        public InMemoryMockRuleRepository()
        {
            Mocks = new List<MockRule>();
        }

        public void Add(MockRule mock) => Mocks.Add(mock);

        public void Remove(MockRule mock) => Mocks.Remove(mock);

        public List<MockRule> GetAllMocks() => Mocks;
    }
}
