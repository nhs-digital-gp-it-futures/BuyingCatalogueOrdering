using System;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public sealed class CreateOrderItemRequest
    {
        public int OrderItemId { get; }

        public Order Order { get; }

        public string OdsCode { get; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public ProvisioningType ProvisioningType { get; }

        public CataloguePriceUnit CataloguePriceUnit { get; }

        public TimeUnit PriceUnit { get; }

        public string CurrencyCode { get; }

        public int Quantity { get; }

        public TimeUnit EstimationPeriod { get; }

        public DateTime? DeliveryDate { get; }

        public decimal? Price { get; }

        public CreateOrderItemRequest(
            Order order, 
            string odsCode,
            CatalogueItemType catalogueItemType,
            CataloguePriceUnit cataloguePriceUnit)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(odsCode));
            }

            Order = order ?? throw new ArgumentNullException(nameof(order));
            OdsCode = odsCode;
            CatalogueItemType = catalogueItemType ?? throw new ArgumentNullException(nameof(catalogueItemType));
            CataloguePriceUnit = cataloguePriceUnit ?? throw new ArgumentNullException(nameof(cataloguePriceUnit));
        }
    }


}
