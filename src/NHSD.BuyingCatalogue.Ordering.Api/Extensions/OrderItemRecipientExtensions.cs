using System.Collections.Generic;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class OrderItemRecipientExtensions
    {
        internal static IReadOnlyList<OrderItemRecipientModel> ToModelList(this IReadOnlyList<OrderItemRecipient> recipients)
        {
            return recipients.OrderBy(r => r.Recipient.Name).Select(r => new OrderItemRecipientModel
            {
                DeliveryDate = r.DeliveryDate,
                Name = r.Recipient.Name,
                OdsCode = r.Recipient.OdsCode,
                Quantity = r.Quantity,
            }).ToList();
        }
    }
}
