using System;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class CreateOrderItemRequestPayloadBuilder
    {
        private bool _hasServiceRecipient;
        private bool _hasItemUnit;
        private bool _hasTimeUnit;

        private string _odsCode;
        private string _catalogueItemId;
        private string _catalogueItemName;
        private string _catalogueSolutionId;
        private DateTime? _deliveryDate;
        private int? _quantity;
        private TimeUnit? _estimationPeriod;
        private CatalogueItemType? _catalogueItemType;
        private ProvisioningType? _provisioningType;
        private CataloguePriceType? _cataloguePriceType;
        private string _currencyCode;
        private string _itemUnitName;
        private string _itemUnitNameDescription;
        private string _timeUnitName;
        private string _timeUnitDescription;
        private decimal? _price;

        private CreateOrderItemRequestPayloadBuilder()
        {
            _hasServiceRecipient = true;
            _hasItemUnit = true;
            _hasTimeUnit = true;

            _odsCode = "ODS1";
            _catalogueItemId = "100001-001";
            _catalogueItemName = Guid.NewGuid().ToString();
            _catalogueItemType = CatalogueItemType.Solution;
            _catalogueSolutionId = null;
            _deliveryDate = new DateTime(2021, 1, 1);
            _quantity = 5;
            _estimationPeriod = TimeUnit.Month;
            _provisioningType = ProvisioningType.OnDemand;
            _cataloguePriceType = CataloguePriceType.Flat;
            _currencyCode = "GBP";
            _itemUnitName = "consultation";
            _itemUnitNameDescription = "per consultation";
            _timeUnitName = "month";
            _timeUnitDescription = "per month";
            _price = 1.5m;
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

        public CreateOrderItemRequestPayloadBuilder WithHasServiceRecipient(bool hasServiceRecipient)
        {
            _hasServiceRecipient = hasServiceRecipient;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithHasItemUnit(bool hasItemUnit)
        {
            _hasItemUnit = hasItemUnit;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithHasTimeUnit(bool hasTimeUnit)
        {
            _hasTimeUnit = hasTimeUnit;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueItemId(string catalogueItemId)
        {
            _catalogueItemId = catalogueItemId;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueItemName(string catalogueItemName)
        {
            _catalogueItemName = catalogueItemName;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueSolutionId(string catalogueSolutionId)
        {
            _catalogueSolutionId = catalogueSolutionId;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCatalogueItemType(CatalogueItemType? catalogueItemType)
        {
            _catalogueItemType = catalogueItemType;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate?.Date;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithQuantity(int? quantity)
        {
            _quantity = quantity;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithEstimationPeriod(TimeUnit? estimationPeriod)
        {
            _estimationPeriod = estimationPeriod;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithProvisioningType(ProvisioningType? provisioningType)
        {
            _provisioningType = provisioningType;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCataloguePriceType(CataloguePriceType? cataloguePriceType)
        {
            _cataloguePriceType = cataloguePriceType;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithCurrencyCode(string currencyCode)
        {
            _currencyCode = currencyCode;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithItemUnitName(string itemUnitName)
        {
            _itemUnitName = itemUnitName;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithItemUnitNameDescription(string itemUnitNameDescription)
        {
            _itemUnitNameDescription = itemUnitNameDescription;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithTimeUnitName(string timeUnitName)
        {
            _timeUnitName = timeUnitName;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithTimeUnitDescription(string timeUnitDescription)
        {
            _timeUnitDescription = timeUnitDescription;
            return this;
        }

        public CreateOrderItemRequestPayloadBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public CreateOrderItemRequestPayload Build()
        {
            return new CreateOrderItemRequestPayload
            {
                HasServiceRecipient = _hasServiceRecipient,
                CatalogueItemType = _catalogueItemType,
                HasItemUnit = _hasItemUnit,
                HasTimeUnit = _hasTimeUnit,
                OdsCode = _odsCode,
                CatalogueItemId = _catalogueItemId,
                CatalogueItemName = _catalogueItemName,
                CatalogueSolutionId = _catalogueSolutionId,
                DeliveryDate = _deliveryDate,
                Quantity = _quantity,
                EstimationPeriod = _estimationPeriod,
                ProvisioningType = _provisioningType,
                CataloguePriceType = _cataloguePriceType,
                CurrencyCode = _currencyCode,
                ItemUnitName = _itemUnitName,
                ItemUnitNameDescription = _itemUnitNameDescription,
                TimeUnitName = _timeUnitName,
                TimeUnitDescription = _timeUnitDescription,
                Price = _price
            };
        }
    }
}
