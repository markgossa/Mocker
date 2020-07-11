using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Contracts
{
    public interface IHistoryQueryProcessor
    {
        Task<HttpResponseMessage> ProcessAsync(Dictionary<string, string> query);
    }
}