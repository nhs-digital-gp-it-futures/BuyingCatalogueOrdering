using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public abstract class CreateOrderItemRequest
    {
        protected CreateOrderItemRequest(Order order, CreateOrderItemModel model, CatalogueItemType itemType)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            // Model validation should ensure Quantity and Price are not null under normal circumstances
            if (model.Quantity is null)
                throw ModelPropertyIsNullException(nameof(model), nameof(CreateOrderItemModel.Quantity));

            if (model.Price is null)
                throw ModelPropertyIsNullException(nameof(model), nameof(CreateOrderItemModel.Price));

            Order = order ?? throw new ArgumentNullException(nameof(order));
            CatalogueItemId = model.CatalogueItemId;
            CatalogueItemType = itemType;
            CatalogueItemName = model.CatalogueItemName;
            ProvisioningType = OrderingEnums.ParseStrictIgnoreCase<ProvisioningType>(model.ProvisioningType);
            CataloguePriceType = OrderingEnums.ParseStrictIgnoreCase<CataloguePriceType>(model.Type);
            CataloguePriceUnitTierName = model.ItemUnit.Name;
            CataloguePriceUnitDescription = model.ItemUnit.Description;
            CurrencyCode = model.CurrencyCode;
            Quantity = model.Quantity.Value;
            EstimationPeriod = OrderingEnums.ParseTimeUnit(model.EstimationPeriod);
            Price = model.Price.Value;
        }

        public Order Order { get; }

        public ServiceRecipient ServiceRecipient { get; protected set; }

        public string OdsCode { get; protected set; }

        public string CatalogueItemId { get; }

        public CatalogueItemType CatalogueItemType { get; }

        public string CatalogueItemName { get; }

        public string CatalogueSolutionId { get; protected set; }

        public ProvisioningType ProvisioningType { get; }

        public CataloguePriceType CataloguePriceType { get; }

        public string CataloguePriceUnitTierName { get; }

        public string CataloguePriceUnitDescription { get; }

        public TimeUnit? PriceTimeUnit { get; protected set; }

        public string CurrencyCode { get; }

        public int Quantity { get; }

        public TimeUnit? EstimationPeriod { get; }

        public DateTime? DeliveryDate { get; protected set; }

        public decimal Price { get; }

        private static ArgumentException ModelPropertyIsNullException(string modelParamName, string propertyName)
        {
            return new ArgumentException($"{modelParamName}.{propertyName} should not be null", modelParamName);
        }
    }
}
