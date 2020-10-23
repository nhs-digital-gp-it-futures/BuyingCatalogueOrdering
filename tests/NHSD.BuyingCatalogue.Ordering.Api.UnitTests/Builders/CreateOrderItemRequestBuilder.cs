using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class CreateOrderItemRequestBuilder
    {
        private Order order;
        private string odsCode;
        private string catalogueItemId;
        private CatalogueItemType catalogueItemType;
        private string catalogueItemName;
        private string catalogueSolutionId;
        private string cataloguePriceTypeName;
        private string provisioningTypeName;
        private string cataloguePriceUnitTierName;
        private string cataloguePriceUnitDescription;
        private TimeUnit? priceTimeUnit;
        private string currencyCode;
        private int quantity;
        private string estimationPeriodName;
        private DateTime? deliveryDate;
        private decimal? price;

        private CreateOrderItemRequestBuilder()
        {
            order = OrderBuilder.Create().Build();
            odsCode = "ODS2";
            catalogueItemId = "10001-001";
            catalogueItemType = CatalogueItemType.AssociatedService;
            catalogueItemName = "Some catalogue item name";
            catalogueSolutionId = "10001-002";
            provisioningTypeName = "Declarative";
            cataloguePriceTypeName = "Flat";
            cataloguePriceUnitTierName = "sms";
            cataloguePriceUnitDescription = "per sms";
            priceTimeUnit = TimeUnit.PerMonth;
            currencyCode = "GBP";
            quantity = 10;
            estimationPeriodName = "year";
            deliveryDate = DateTime.UtcNow;
            price = 2;
        }

        public static CreateOrderItemRequestBuilder Create() => new CreateOrderItemRequestBuilder();

        public CreateOrderItemRequestBuilder WithOrder(Order o)
        {
            order = o;
            return this;
        }

        public CreateOrderItemRequestBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCatalogueItemId(string id)
        {
            catalogueItemId = id;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCatalogueItemType(CatalogueItemType type)
        {
            catalogueItemType = type;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCatalogueItemName(string name)
        {
            catalogueItemName = name;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCatalogueSolutionId(string id)
        {
            catalogueSolutionId = id;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCataloguePriceTypeName(string priceTypeName)
        {
            cataloguePriceTypeName = priceTypeName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithProvisioningTypeName(string typeName)
        {
            provisioningTypeName = typeName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCataloguePriceUnitTierName(string tierName)
        {
            cataloguePriceUnitTierName = tierName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCataloguePriceUnitDescription(string description)
        {
            cataloguePriceUnitDescription = description;
            return this;
        }

        public CreateOrderItemRequestBuilder WithPriceTimeUnitName(TimeUnit? unit)
        {
            priceTimeUnit = unit;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCurrencyCode(string code)
        {
            currencyCode = code;
            return this;
        }

        public CreateOrderItemRequestBuilder WithQuantity(int number)
        {
            quantity = number;
            return this;
        }

        public CreateOrderItemRequestBuilder WithEstimationPeriodName(string periodName)
        {
            estimationPeriodName = periodName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithDeliveryDate(DateTime? date)
        {
            deliveryDate = date;
            return this;
        }

        public CreateOrderItemRequestBuilder WithPrice(decimal? cost)
        {
            price = cost;
            return this;
        }

        public CreateOrderItemRequest Build() =>
            new CreateOrderItemRequest(
                order,
                odsCode,
                catalogueItemId,
                catalogueItemType,
                catalogueItemName,
                catalogueSolutionId,
                provisioningTypeName,
                cataloguePriceTypeName,
                cataloguePriceUnitTierName,
                cataloguePriceUnitDescription,
                priceTimeUnit,
                currencyCode,
                quantity,
                estimationPeriodName,
                deliveryDate,
                price);
    }
}
