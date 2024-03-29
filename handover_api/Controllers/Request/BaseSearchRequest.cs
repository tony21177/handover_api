﻿using handover_api.Service.ValueObject;

namespace handover_api.Controllers.Request
{
    public class BaseSearchRequest
    {
        public string? SearchString { get; set; }

        public PaginationCondition PaginationCondition { get; set; } = new PaginationCondition();
    }
}
