using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Application
{
    public static class OrderExtensions
    {
        public static IReadOnlyList<FlattenedOrderItem> FlattenOrderItems(this Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var items = new List<FlattenedOrderItem>();
            var i = 1;

            foreach (var orderItem in order.OrderItems)
            {
                foreach (var recipient in orderItem.OrderItemRecipients)
                {
                    var flattenedItem = new FlattenedOrderItem
                    {
                        CatalogueItem = orderItem.CatalogueItem,
                        CataloguePriceType = orderItem.CataloguePriceType,
                        CurrencyCode = orderItem.CurrencyCode,
                        DeliveryDate = recipient.DeliveryDate,
                        EstimationPeriod = orderItem.EstimationPeriod,
                        ItemId = i,
                        Order = order,
                        Price = orderItem.Price,
                        PriceTimeUnit = orderItem.PriceTimeUnit,
                        PricingUnit = orderItem.PricingUnit,
                        ProvisioningType = orderItem.ProvisioningType,
                        Quantity = recipient.Quantity,
                        Recipient = recipient.Recipient,
                    };

                    items.Add(flattenedItem);
                    i++;
                }
            }

            return items;
        }
    }
}
