using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetOrderItemModel
    {
        public GetOrderItemModel()
        {
        }

        internal GetOrderItemModel(OrderItem orderItem)
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

        public string CatalogueItemId { get; init; }

        public string CatalogueItemType { get; init; }

        public string CatalogueItemName { get; init; }

        public string CurrencyCode { get; init; }

        public DateTime? DefaultDeliveryDate { get; init; }

        public string EstimationPeriod { get; init; }

        public ItemUnitModel ItemUnit { get; init; }

        public decimal? Price { get; init; }

        public string ProvisioningType { get; init; }

        public IReadOnlyCollection<OrderItemRecipientModel> ServiceRecipients { get; init; }

        public TimeUnitModel TimeUnit { get; init; }

        public string Type { get; init; }
    }
}
