using Mocker.Application.Services;
using Mocker.Domain.Models.Http;
using Mocker.Functions.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Functions.Services
{
    public class HttpRuleRequestProcessor : IHttpRuleRequestProcessor
    {
        private readonly IHttpRuleService _httpRuleService;

        public HttpRuleRequestProcessor(IHttpRuleService httpRuleService)
        {
            _httpRuleService = httpRuleService;
        }

        public async Task AddAsync(HttpRuleRequest httpRuleRequest)
        {
            HttpMethod? httpMethod = null;
            if (httpRuleRequest.Filter.Method != null)
            {
                httpMethod = new HttpMethod(httpRuleRequest.Filter.Method);
            }

            var httpFilter = new HttpFilter(httpMethod, httpRuleRequest.Filter.Body,
                httpRuleRequest.Filter.Route, httpRuleRequest.Filter.Query, httpRuleRequest.Filter.Headers);

            var httpAction = new HttpAction(httpRuleRequest.Action.StatusCode, httpRuleRequest.Action.Body,
                httpRuleRequest.Action.Headers, httpRuleRequest.Action.Delay);

            await _httpRuleService.AddAsync(new HttpRule(httpFilter, httpAction));
        }

        public async Task<HttpRuleResponse> GetAllAsync()
        {
            var rules = await _httpRuleService.GetAllAsync();
            var httpRules = new HttpRuleResponse();
            foreach (var rule in rules)
            {
                var httpRule = new HttpRuleRequest()
                {
                    Id = rule.Id,
                    Action = new HttpRuleActionRequest
                    {
                        Body = rule.HttpAction.Body,
                        Delay = rule.HttpAction.Delay,
                        Headers = rule.HttpAction.Headers,
                        StatusCode = rule.HttpAction.StatusCode
                    },
                    Filter = new HttpRuleFilterRequest
                    {
                        Body = rule.HttpFilter.Body,
                        Headers = rule.HttpFilter.Headers,
                        Method = rule.HttpFilter.Method?.ToString(),
                        Query = rule.HttpFilter.Query,
                        Route = rule.HttpFilter.Route
                    }
                };

                httpRules.Rules.Add(httpRule);
            }

            return httpRules;
        }

        public async Task RemoveAllAsync() => await _httpRuleService.RemoveAllAsync();

        public bool Validate(HttpRuleRequest httpRuleRequest, out List<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(httpRuleRequest, new ValidationContext(httpRuleRequest), validationResults);
        }
    }
}
