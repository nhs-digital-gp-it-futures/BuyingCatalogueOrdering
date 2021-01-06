using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderItemModel
    {
        internal OrderItemModel(
            string orderId,
            OrderItem orderItem,
            string serviceInstanceId)
        {
            ItemId = $"{orderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}";
            ServiceRecipientsOdsCode = orderItem.OdsCode;
            CataloguePriceType = orderItem.CataloguePriceType.ToString();
            CatalogueItemType = orderItem.CatalogueItemType.ToString();
            CatalogueItemName = orderItem.CatalogueItemName;
            ProvisioningType = orderItem.ProvisioningType.ToString();
            ItemUnitDescription = orderItem.CataloguePriceUnit.Description;
            TimeUnitDescription = orderItem.PriceTimeUnit?.Description();
            QuantityPeriodDescription = orderItem.EstimationPeriod?.Description();
            DeliveryDate = orderItem.DeliveryDate;
            Price = orderItem.Price;
            Quantity = orderItem.Quantity;
            CostPerYear = orderItem.CalculateTotalCostPerYear();
            ServiceInstanceId = serviceInstanceId;
        }

        public string ItemId { get; init; }

        public string CataloguePriceType { get; init; }

        public string CatalogueItemType { get; init; }

        public string CatalogueItemName { get; init; }

        public string ProvisioningType { get; init; }

        public decimal? Price { get; init; }

        public string ItemUnitDescription { get; init; }

        public string TimeUnitDescription { get; init; }

        public int Quantity { get; init; }

        public string QuantityPeriodDescription { get; init; }

        public DateTime? DeliveryDate { get; init; }

        public decimal CostPerYear { get; init; }

        public string ServiceRecipientsOdsCode { get; init; }

        public string ServiceInstanceId { get; init; }
    }
}
