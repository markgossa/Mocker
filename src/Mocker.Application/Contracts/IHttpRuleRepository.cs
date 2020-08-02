﻿using Mocker.Domain.Models.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mocker.Application.Contracts
{
    public interface IHttpRuleRepository
    {
        Task AddAsync(HttpRule httpRule);

        Task RemoveAllAsync();

        Task<IEnumerable<HttpRule>> GetAllAsync();
    }
}