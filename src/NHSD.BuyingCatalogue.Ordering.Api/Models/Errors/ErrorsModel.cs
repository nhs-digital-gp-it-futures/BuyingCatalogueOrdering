﻿using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class ErrorsModel
    {
        public ErrorsModel(IEnumerable<ErrorModel> errors)
        {
            Errors = errors;
        }

        public IEnumerable<ErrorModel> Errors { get; }
    }
}
