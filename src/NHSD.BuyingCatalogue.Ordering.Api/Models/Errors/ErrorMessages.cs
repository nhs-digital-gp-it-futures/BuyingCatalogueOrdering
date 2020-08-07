﻿using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Errors
{
    internal static class ErrorMessages
    {
        public static ErrorModel InvalidOrderStatus() => new ErrorModel("InvalidStatus", "Status");

        public static ErrorModel OrderNotComplete() => new ErrorModel("OrderNotComplete", nameof(Order));
    }
}
