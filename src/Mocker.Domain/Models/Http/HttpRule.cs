﻿namespace Mocker.Domain.Models.Http
{
    public class HttpRule : MockRule
    {
        public HttpFilter HttpFilter { get; }
        public HttpAction HttpAction { get; }

        public HttpRule(HttpFilter httpFilter, HttpAction httpAction, int id = 0)
        {
            HttpFilter = httpFilter;
            HttpAction = httpAction;
            Id = id;
        }
    }
}
