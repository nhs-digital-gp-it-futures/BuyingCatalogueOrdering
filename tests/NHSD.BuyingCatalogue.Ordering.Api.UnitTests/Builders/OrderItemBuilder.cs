using System;
using System.Reflection;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderItemBuilder
    {
        private int? _orderItemId;
        private string _odsCode;
        private string _catalogueItemId; 
        private CatalogueItemType _catalogueItemType; 
        private string _catalogueItemName;
        private ProvisioningType _provisioningType; 
        private CataloguePriceType _cataloguePriceType;
        private CataloguePriceUnit _cataloguePriceUnit;
        private TimeUnit _priceTimeUnit;
        private string _currencyCode; 
        private int _quantity;
        private TimeUnit _estimationPeriod;
        private DateTime? _deliveryDate;
        private decimal? _price;

        private OrderItemBuilder()
        {
            _odsCode = "ODS1";
            _catalogueItemId = "1000-001"; 
            _catalogueItemType = CatalogueItemType.Solution;
            _catalogueItemName = Guid.NewGuid().ToString();
            _provisioningType = ProvisioningType.Patient; 
            _cataloguePriceType = CataloguePriceType.Flat;
            _cataloguePriceUnit = CataloguePriceUnit.Create("patients", "per patient");
            _priceTimeUnit = null;
            _currencyCode = "GBP"; 
            _quantity = 10;
            _estimationPeriod = TimeUnit.PerYear;
            _deliveryDate = DateTime.UtcNow;
            _price = 2.000m;
        }

        public static OrderItemBuilder Create() => 
            new OrderItemBuilder();

        public OrderItemBuilder WithOrderItemId(int orderItemId)
        {
            _orderItemId = orderItemId;
            return this;
        }

        public OrderItemBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        public OrderItemBuilder WithCatalogueItemId(string catalogueItemId)
        {
            _catalogueItemId = catalogueItemId;
            return this;
        }

        public OrderItemBuilder WithCatalogueItemType(CatalogueItemType catalogueItemType)
        {
            _catalogueItemType = catalogueItemType;
            return this;
        }

        public OrderItemBuilder WithCatalogueItemName(string catalogueItemName)
        {
            _catalogueItemName = catalogueItemName;
            return this;
        }

        public OrderItemBuilder WithProvisioningType(ProvisioningType provisioningType)
        {
            _provisioningType = provisioningType;
            return this;
        }

        public OrderItemBuilder WithCataloguePriceType(CataloguePriceType cataloguePriceType)
        {
            _cataloguePriceType = cataloguePriceType;
            return this;
        }

        public OrderItemBuilder WithCataloguePriceUnit(CataloguePriceUnit cataloguePriceUnit)
        {
            _cataloguePriceUnit = cataloguePriceUnit;
            return this;
        }

        public OrderItemBuilder WithPriceTimeUnit(TimeUnit priceTimeUnit)
        {
            _priceTimeUnit = priceTimeUnit;
            return this;
        }

        public OrderItemBuilder WithCurrencyCode(string currencyCode)
        {
            _currencyCode = currencyCode;
            return this;
        }

        public OrderItemBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        public OrderItemBuilder WithEstimationPeriod(TimeUnit estimationPeriod)
        {
            _estimationPeriod = estimationPeriod;
            return this;
        }

        public OrderItemBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate;
            return this;
        }

        public OrderItemBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public OrderItem Build()
        {
            var orderItem = new OrderItem(
                _odsCode,
                _catalogueItemId,
                _catalogueItemType,
                _catalogueItemName,
                null,
                _provisioningType,
                _cataloguePriceType,
                _cataloguePriceUnit,
                _priceTimeUnit,
                _currencyCode,
                _quantity,
                _estimationPeriod,
                _deliveryDate,
                _price);

            if (_orderItemId.HasValue)
            {
                var fieldInfo = orderItem.GetType().GetField("_orderItemId", BindingFlags.Instance|BindingFlags.NonPublic);
                fieldInfo?.SetValue(orderItem, _orderItemId.Value);
            }

            return orderItem;
        }
    }
}
