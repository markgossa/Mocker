using System.Collections.Generic;

namespace Mocker.Functions.Models
{
    public class HttpRuleResponse
    {
        public List<HttpRuleRequest> Rules { get; set; }

        public HttpRuleResponse()
        {
            Rules = new List<HttpRuleRequest>();
        }
    }
}
