using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class CreateOrderItemRequestBuilder
    {
        private Order _order;
        private string _odsCode;
        private string _catalogueItemId;
        private CatalogueItemType _catalogueItemType;
        private string _catalogueItemName;
        private string _cataloguePriceTypeName;
        private string _provisioningTypeName;
        private string _cataloguePriceUnitTierName;
        private string _cataloguePriceUnitDescription;
        private string _priceTimeUnitName;
        private string _currencyCode;
        private int _quantity;
        private string _estimationPeriodName;
        private DateTime? _deliveryDate;
        private decimal? _price;

        private CreateOrderItemRequestBuilder()
        {
            _order = OrderBuilder.Create().Build();
            _odsCode = "ODS2";
            _catalogueItemId = "10001-001";
            _catalogueItemType = CatalogueItemType.AssociatedService;
            _catalogueItemName = "Some catalogue item name";
            _provisioningTypeName = "Declarative";
            _cataloguePriceTypeName = "Flat";
            _cataloguePriceUnitTierName = "sms";
            _cataloguePriceUnitDescription = "per sms";
            _priceTimeUnitName = "month";
            _currencyCode = "EUR";
            _quantity = 10;
            _estimationPeriodName = "year";
            _deliveryDate = DateTime.UtcNow;
            _price = 2;
        }

        public static CreateOrderItemRequestBuilder Create() => new CreateOrderItemRequestBuilder();

        public CreateOrderItemRequestBuilder WithOrder(Order order)
        {
            _order = order;
            return this;
        }

        public CreateOrderItemRequestBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCatalogueItemId(string catalogueItemId)
        {
            _catalogueItemId = catalogueItemId;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCatalogueItemType(CatalogueItemType catalogueItemType)
        {
            _catalogueItemType = catalogueItemType;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCatalogueItemName(string catalogueItemName)
        {
            _catalogueItemName = catalogueItemName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCataloguePriceTypeName(string cataloguePriceTypeName)
        {
            _cataloguePriceTypeName = cataloguePriceTypeName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithProvisioningTypeName(string provisioningTypeName)
        {
            _provisioningTypeName = provisioningTypeName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCataloguePriceUnitTierName(string cataloguePriceUnitTierName)
        {
            _cataloguePriceUnitTierName = cataloguePriceUnitTierName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCataloguePriceUnitDescription(string cataloguePriceUnitDescription)
        {
            _cataloguePriceUnitDescription = cataloguePriceUnitDescription;
            return this;
        }

        public CreateOrderItemRequestBuilder WithPriceTimeUnitName(string priceTimeUnitName)
        {
            _priceTimeUnitName = priceTimeUnitName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithCurrencyCode(string currencyCode)
        {
            _currencyCode = currencyCode;
            return this;
        }

        public CreateOrderItemRequestBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        public CreateOrderItemRequestBuilder WithEstimationPeriodName(string estimationPeriodName)
        {
            _estimationPeriodName = estimationPeriodName;
            return this;
        }

        public CreateOrderItemRequestBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate;
            return this;
        }

        public CreateOrderItemRequestBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public CreateOrderItemRequest Build() =>
            new CreateOrderItemRequest(
                _order,
                _odsCode,
                _catalogueItemId,
                _catalogueItemType,
                _catalogueItemName,
                _provisioningTypeName,
                _cataloguePriceTypeName,
                _cataloguePriceUnitTierName,
                _cataloguePriceUnitDescription,
                _priceTimeUnitName,
                _currencyCode,
                _quantity,
                _estimationPeriodName,
                _deliveryDate,
                _price
            );
    }
}
