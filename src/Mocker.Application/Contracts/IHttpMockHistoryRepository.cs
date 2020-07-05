﻿using Mocker.Application.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mocker.Application.Contracts
{
    public interface IHttpMockHistoryRepository
    {
        Task AddAsync(HttpRequestDetails httpRequestDetails);
        Task<List<HttpRequestDetails>> FindAsync(HttpMockHistoryFilter httpMockHistoryFilter);
        Task<List<HttpRequestDetails>> FindByMethodAsync(HttpMethod httpMethod);
        Task DeleteAllAsync();
    }
}
