using System.Collections.Generic;

namespace Mocker.Functions.Models
{
    public class HttpRuleFilterRequest
    {
        public string? Body { get; set; }
        public Dictionary<string, List<string>>? Headers { get; set; }
        public bool? IgnoreHeaders { get; set; }
        public string? Method { get; set; }
        public Dictionary<string, string>? Query { get; set; }
        public string? Route { get; set; }
    }
}
