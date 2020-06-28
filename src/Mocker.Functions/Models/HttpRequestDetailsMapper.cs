using Microsoft.AspNetCore.Http;
using Mocker.Application;
using Mocker.Functions.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mocker.Functions.Models
{
    public class HttpRequestDetailsMapper : IMapper<HttpRequestObject, Task<HttpRequestDetails>>
    {
        public async Task<HttpRequestDetails> Map(HttpRequestObject httpRequest)
        {
            using var streamReader = new StreamReader(httpRequest.BodyStream);
            var body = await streamReader.ReadToEndAsync();

            var headers = ConvertHeadersToDictionary(httpRequest.Headers);
                
            return new HttpRequestDetails(httpRequest.Method, httpRequest.Route, body, headers, httpRequest.Query);
        }

        private Dictionary<string, List<string>> ConvertHeadersToDictionary(IHeaderDictionary headers) => 
            headers.ToDictionary(h => h.Key, h => new List<string>(h.Value));
    }
}
