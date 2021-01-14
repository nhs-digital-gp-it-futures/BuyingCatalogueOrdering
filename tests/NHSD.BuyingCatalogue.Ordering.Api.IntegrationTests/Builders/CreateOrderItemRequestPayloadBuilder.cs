using System;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class CreateOrderItemRequestPayloadBuilder
    {
        private bool hasServiceRecipient;
        private bool hasItemUnit;
        private bool hasTimeUnit;

        private string odsCode;
        private string catalogueItemId;
        private string catalogueItemName;
        private string catalogueSolutionId;
        private DateTime? deliveryDate;
        private int? quantity;
        private TimeUnit? estimationPeriod;
        private CatalogueItemType? catalogueItemType;
        private ProvisioningType? provisioningType;
        private CataloguePriceType? cataloguePriceType;
        private string currencyCode;
        private string itemUnitName;
        private string itemUnitNameDescription;
        private string timeUnitName;
        private string timeUnitDescription;
        private decimal? price;

        private CreateOrderItemRequestPayloadBuilder()
        {
            hasServiceRecipient = true;
            hasItemUnit = true;
            hasTimeUnit = true;

            odsCode = "ODS1";
            catalogueItemId = "100001-001";
            catalogueItemName = Guid.NewGuid().ToString();
            catalogueItemType = CatalogueItemType.Solution;
            catalogueSolutionId = null;
            deliveryDate = new DateTime(2021, 1, 1);
            quantity = 5;
            estimationPeriod = TimeUnit.Month;
            provisioningType = ProvisioningType.OnDemand;
            cataloguePriceType = CataloguePriceType.Flat;
            currencyCode = "GBP";
            itemUnitName = "consultation";
            itemUnitNameDescription = "per consultation";
            timeUnitName = "month";
            timeUnitDescription = "per month";
            price = 1.5m;
        }

        public static CreateOrderItemRequestPayloadBuilder CreateSolution() =>
            new CreateOrderItemRequestPayloadBuilder()
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .WithCatalogueSolutionId(null);

        public static CreateOrderItemRequestPayloadBuilder CreateAdditionalService() =>
            new CreateOrderItemRequestPayloadBuilder()
                .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                .WithCatalogueSolutionId("123")
                .WithDeliveryDate(null);

        public static CreateOrderItemRequestPayloadBuilder CreateAssociatedService() =>
            new CreateOrderItemRequestPayloadBuilder()
                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                .WithHasServiceRecipient(false)
                .WithHasTimeUnit(false)
                .WithDeliveryDate(null);

        public CreateOrderItemRequestPayloadBuilder WithHasServiceRecipient(bool value)
        {
            hasServiceRecipient = value;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithHasItemUnit(bool value)
        {
            hasItemUnit = value;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithHasTimeUnit(bool value)
        {
            hasTimeUnit = value;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueItemId(string id)
        {
            catalogueItemId = id;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueItemName(string name)
        {
            catalogueItemName = name;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueSolutionId(string id)
        {
            catalogueSolutionId = id;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueItemType(CatalogueItemType? type)
        {
            catalogueItemType = type;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithDeliveryDate(DateTime? date)
        {
            deliveryDate = date?.Date;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithQuantity(int? number)
        {
            quantity = number;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithEstimationPeriod(TimeUnit? period)
        {
            estimationPeriod = period;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithProvisioningType(ProvisioningType? type)
        {
            provisioningType = type;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCataloguePriceType(CataloguePriceType? priceType)
        {
            cataloguePriceType = priceType;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCurrencyCode(string code)
        {
            currencyCode = code;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithItemUnitName(string name)
        {
            itemUnitName = name;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithItemUnitNameDescription(string description)
        {
            itemUnitNameDescription = description;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithTimeUnitName(string name)
        {
            timeUnitName = name;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithTimeUnitDescription(string description)
        {
            timeUnitDescription = description;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithPrice(decimal? cost)
        {
            price = cost;
            return this;
        }

        public CreateOrderItemRequestPayload Build()
        {
            return new()
            {
                HasServiceRecipient = hasServiceRecipient,
                CatalogueItemType = catalogueItemType,
                HasItemUnit = hasItemUnit,
                HasTimeUnit = hasTimeUnit,
                OdsCode = odsCode,
                CatalogueItemId = catalogueItemId,
                CatalogueItemName = catalogueItemName,
                CatalogueSolutionId = catalogueSolutionId,
                DeliveryDate = deliveryDate,
                Quantity = quantity,
                EstimationPeriod = estimationPeriod,
                ProvisioningType = provisioningType,
                CataloguePriceType = cataloguePriceType,
                CurrencyCode = currencyCode,
                ItemUnitName = itemUnitName,
                ItemUnitNameDescription = itemUnitNameDescription,
                TimeUnitName = timeUnitName,
                TimeUnitDescription = timeUnitDescription,
                Price = price,
            };
        }
    }
}
