using System.ComponentModel.DataAnnotations;
#nullable disable

namespace Mocker.Functions.Models
{
    public class HttpRuleRequest
    {
        [Required]
        public HttpRuleFilterRequest Filter { get; set; }
        
        [Required]
        public HttpRuleActionRequest Action { get; set; }
    }
}
