using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Contracts
{
    public interface IHttpHistoryQueryService
    {
        Task<HttpResponseMessage> ExecuteQueryAsync(Dictionary<string, string> query);
    }
}