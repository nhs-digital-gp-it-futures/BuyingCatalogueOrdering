using System;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class CreateCatalogueSolutionOrderItemRequestPayloadBuilder
    {
        private bool _hasServiceRecipient;
        private bool _hasItemUnit;
        private string _odsCode;
        private string _catalogueSolutionId;
        private string _catalogueSolutionName;
        private DateTime? _deliveryDate;
        private int? _quantity;
        private TimeUnit? _estimationPeriod;
        private ProvisioningType? _provisioningType;
        private CataloguePriceType? _cataloguePriceType;
        private string _currencyCode;
        private string _itemUnitName;
        private string _itemUnitNameDescription;
        private decimal? _price;

        private CreateCatalogueSolutionOrderItemRequestPayloadBuilder()
        {
            _hasServiceRecipient = true;
            _hasItemUnit = true;
            _odsCode = "ODS1";
            _catalogueSolutionId = "100001-001";
            _catalogueSolutionName = Guid.NewGuid().ToString();
            _deliveryDate = new DateTime(2021, 1, 1);
            _quantity = 5;
            _estimationPeriod = TimeUnit.Month;
            _provisioningType = ProvisioningType.OnDemand;
            _cataloguePriceType = CataloguePriceType.Flat;
            _currencyCode = "GBP";
            _itemUnitName = "consultation";
            _itemUnitNameDescription = "per consultation";
            _price = 1.5m;
        }

        public static CreateCatalogueSolutionOrderItemRequestPayloadBuilder Create() => 
            new CreateCatalogueSolutionOrderItemRequestPayloadBuilder();

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithHasServiceRecipient(bool hasServiceRecipient)
        {
            _hasServiceRecipient = hasServiceRecipient;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithHasItemUnit(bool hasItemUnit)
        {
            _hasItemUnit = hasItemUnit;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithCatalogueSolutionId(string catalogueSolutionId)
        {
            _catalogueSolutionId = catalogueSolutionId;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithCatalogueSolutionName(string catalogueSolutionName)
        {
            _catalogueSolutionName = catalogueSolutionName;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate?.Date;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithQuantity(int? quantity)
        {
            _quantity = quantity;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithEstimationPeriod(TimeUnit? estimationPeriod)
        {
            _estimationPeriod = estimationPeriod;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithProvisioningType(ProvisioningType? provisioningType)
        {
            _provisioningType = provisioningType;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithCataloguePriceType(CataloguePriceType? cataloguePriceType)
        {
            _cataloguePriceType = cataloguePriceType;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithCurrencyCode(string currencyCode)
        {
            _currencyCode = currencyCode;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithItemUnitName(string itemUnitName)
        {
            _itemUnitName = itemUnitName;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithItemUnitNameDescription(string itemUnitNameDescription)
        {
            _itemUnitNameDescription = itemUnitNameDescription;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayloadBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public CreateCatalogueSolutionOrderItemRequestPayload Build()
        {
            return new CreateCatalogueSolutionOrderItemRequestPayload
            {
                HasServiceRecipient = _hasServiceRecipient,
                HasItemUnit = _hasItemUnit,
                OdsCode = _odsCode,
                CatalogueSolutionId = _catalogueSolutionId,
                CatalogueSolutionName = _catalogueSolutionName,
                DeliveryDate = _deliveryDate,
                Quantity = _quantity,
                EstimationPeriod = _estimationPeriod,
                ProvisioningType = _provisioningType,
                CataloguePriceType = _cataloguePriceType,
                CurrencyCode = _currencyCode,
                ItemUnitName = _itemUnitName,
                ItemUnitNameDescription = _itemUnitNameDescription,
                Price = _price
            };
        }
    }
}
