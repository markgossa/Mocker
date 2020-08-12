using System.Collections.Generic;
using System.Net;

namespace Mocker.Functions.Models
{
    public class HttpRuleActionRequest
    {
        public string? Body { get; set; }
        public int Delay { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, List<string>>? Headers { get; set; }
    }
}
