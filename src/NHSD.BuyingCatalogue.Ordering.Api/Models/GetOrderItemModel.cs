using System;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetOrderItemModel
    {
        public GetOrderItemModel(OrderItem orderItem, ServiceRecipient serviceRecipient)
        {
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem));

            OrderItemId = orderItem.OrderItemId;
            ServiceRecipient = serviceRecipient != null ? new ServiceRecipientModel
            {
                OdsCode = serviceRecipient.OdsCode,
                Name = serviceRecipient.Name
            } : null;
            CatalogueItemType = orderItem.CatalogueItemType.Name;
            CatalogueItemName = orderItem.CatalogueItemName;
            CatalogueItemId = orderItem.CatalogueItemId;
            CurrencyCode = orderItem.CurrencyCode;
            DeliveryDate = orderItem.DeliveryDate;
            ItemUnit = new ItemUnitModel
            {
                Name = orderItem.CataloguePriceUnit.Name,
                Description = orderItem.CataloguePriceUnit.Description
            };
            Price = orderItem.Price;
            ProvisioningType = orderItem.ProvisioningType.Name;
            Quantity = orderItem.Quantity;
            TimeUnit = orderItem.PriceTimeUnit?.ToModel();
            Type = orderItem.CataloguePriceType.Name;
            EstimationPeriod = orderItem.EstimationPeriod?.Name;
        }

        public int OrderItemId { get; set; }

        public string CatalogueItemId { get; set; }

        public string CatalogueItemType { get; set; }

        public string CatalogueItemName { get; set; }

        public string CataloguePriceType { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public ItemUnitModel ItemUnit { get; set; }

        public decimal? Price { get; set; }

        public string ProvisioningType { get; set; }

        public int Quantity { get; set; }

        public ServiceRecipientModel ServiceRecipient { get; set; }

        public TimeUnitModel TimeUnit { get; set; }

        public string Type { get; set; }

        public string EstimationPeriod { get; set; }
    }
}
