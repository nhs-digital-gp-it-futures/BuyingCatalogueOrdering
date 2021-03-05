using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class OrderItemRecipientModel : IServiceRecipient
    {
        public DateTime? DeliveryDate { get; init; }

        public string Name { get; init; }

        public string OdsCode { get; init; }

        public int? Quantity { get; init; }
    }
}
