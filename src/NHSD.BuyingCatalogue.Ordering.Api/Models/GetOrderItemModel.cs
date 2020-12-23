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
                Name = serviceRecipient.Name,
            } : null;
            CatalogueItemType = orderItem.CatalogueItemType.ToString();
            CatalogueItemName = orderItem.CatalogueItemName;
            CatalogueItemId = orderItem.CatalogueItemId;
            CurrencyCode = orderItem.CurrencyCode;
            DeliveryDate = orderItem.DeliveryDate;
            ItemUnit = new ItemUnitModel
            {
                Name = orderItem.CataloguePriceUnit.Name,
                Description = orderItem.CataloguePriceUnit.Description,
            };
            Price = orderItem.Price;
            ProvisioningType = orderItem.ProvisioningType.ToString();
            Quantity = orderItem.Quantity;
            TimeUnit = orderItem.PriceTimeUnit?.ToModel();
            Type = orderItem.CataloguePriceType.ToString();
            EstimationPeriod = orderItem.EstimationPeriod?.Name();
        }

        public GetOrderItemModel(
            OrderItem orderItem,
            ServiceRecipient serviceRecipient,
            string serviceInstanceId)
            : this(orderItem, serviceRecipient)
        {
            ServiceInstanceId = serviceInstanceId;
        }

        public int OrderItemId { get; }

        public string CatalogueItemId { get; }

        public string CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public string CurrencyCode { get; }

        public DateTime? DeliveryDate { get; }

        public ItemUnitModel ItemUnit { get; }

        public decimal? Price { get; }

        public string ProvisioningType { get; }

        public int Quantity { get; }

        public ServiceRecipientModel ServiceRecipient { get; }

        public string ServiceInstanceId { get; }

        public TimeUnitModel TimeUnit { get; }

        public string Type { get; }

        public string EstimationPeriod { get; }
    }
}
