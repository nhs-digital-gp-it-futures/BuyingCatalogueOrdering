using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetOrderItemModel
    {
        public GetOrderItemModel(OrderItem orderItem)
        {
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem));

            CatalogueItemId = orderItem.CatalogueItem.Id.ToString();
            CatalogueItemName = orderItem.CatalogueItem.Name;
            CatalogueItemType = orderItem.CatalogueItem.CatalogueItemType.ToString();
            CurrencyCode = orderItem.CurrencyCode;
            DefaultDeliveryDate = orderItem.DefaultDeliveryDate;
            EstimationPeriod = orderItem.EstimationPeriod?.Name();
            ItemUnit = new ItemUnitModel
            {
                Name = orderItem.PricingUnit.Name,
                Description = orderItem.PricingUnit.Description,
            };
            Price = orderItem.Price;
            ProvisioningType = orderItem.ProvisioningType.ToString();
            ServiceRecipients = orderItem.OrderItemRecipients.ToModelList();
            TimeUnit = orderItem.PriceTimeUnit?.ToModel();
            Type = orderItem.CataloguePriceType.ToString();
        }

        public string CatalogueItemId { get; }

        public string CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public string CurrencyCode { get; }

        public DateTime? DefaultDeliveryDate { get; }

        public string EstimationPeriod { get; }

        public ItemUnitModel ItemUnit { get; }

        public decimal? Price { get; }

        public string ProvisioningType { get; }

        public IReadOnlyCollection<OrderItemRecipientModel> ServiceRecipients { get; }

        public TimeUnitModel TimeUnit { get; }

        public string Type { get; }
    }
}
