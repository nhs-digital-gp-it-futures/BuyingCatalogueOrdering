using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder
{
    public sealed class CompleteOrderRequest
    {
        public CompleteOrderRequest(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }

        public Order Order { get; }
    }
}
