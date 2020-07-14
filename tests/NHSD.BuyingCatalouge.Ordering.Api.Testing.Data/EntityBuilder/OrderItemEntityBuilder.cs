using System;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class OrderItemEntityBuilder
    {
        private int _orderItemId;
        private string _orderId;
        private string _odsCode;
        private string _catalogueItemId;
        private CatalogueItemType _catalogueItemType;
        private string _catalogueItemName;
        private DateTime? _deliveryDate;
        private int _quantity;
        private TimeUnit? _estimationPeriod;
        private ProvisioningType _provisioningType;
        private CataloguePriceType _cataloguePriceType;
        private string _currencyCode;
        private TimeUnit? _timeUnit;
        private string _pricingUnitTierName;
        private string _pricingUnitName;
        private string _pricingUnitDescription;
        private decimal? _price;
        private DateTime _created;
        private readonly DateTime _lastUpdated;

        private OrderItemEntityBuilder(
            string orderId,
            string odsCode,
            string catalogueItemId,
            CatalogueItemType catalogueItemType,
            string catalogueItemName,
            DateTime? deliveryDate,
            int quantity,
            TimeUnit? estimationPeriod,
            ProvisioningType provisioningType,
            CataloguePriceType cataloguePriceType,
            string currencyCode,
            TimeUnit? timeUnit,
            string pricingUnitTierName,
            string pricingUnitName,
            string pricingUnitDescription,
            decimal? price,
            DateTime created,
            DateTime lastUpdated)
        {
            _orderId = orderId;
            _odsCode = odsCode;
            _catalogueItemId = catalogueItemId;
            _catalogueItemType = catalogueItemType;
            _catalogueItemName = catalogueItemName;
            _deliveryDate = deliveryDate;
            _quantity = quantity;
            _estimationPeriod = estimationPeriod;
            _provisioningType = provisioningType;
            _cataloguePriceType = cataloguePriceType;
            _currencyCode = currencyCode;
            _timeUnit = timeUnit;
            _pricingUnitTierName = pricingUnitTierName;
            _pricingUnitName = pricingUnitName;
            _pricingUnitDescription = pricingUnitDescription;
            _price = price;
            _created = created;
            _lastUpdated = lastUpdated;
        }

        private OrderItemEntityBuilder() : this(
            "C10000-001",
            "ODS1",
            "100001-001",
            CatalogueItemType.Solution,
            Guid.NewGuid().ToString(),
            DateTime.UtcNow.Date,
            5,
            TimeUnit.Month,
            ProvisioningType.OnDemand,
            CataloguePriceType.Flat,
            "GBP",
            null,
            "Tier",
            "Price Name",
            "per consultation",
            1.5m,
            DateTime.UtcNow,
            DateTime.UtcNow)
        {
        }

        public static OrderItemEntityBuilder Create() =>
            new OrderItemEntityBuilder();

        public static OrderItemEntityBuilder Create(OrderItemEntity orderItemEntity)
        {
            if (orderItemEntity is null)
                throw new ArgumentNullException(nameof(orderItemEntity));

            return new OrderItemEntityBuilder(
                orderItemEntity.OrderId,
                orderItemEntity.OdsCode,
                orderItemEntity.CatalogueItemId,
                orderItemEntity.CatalogueItemType,
                orderItemEntity.CatalogueItemName,
                orderItemEntity.DeliveryDate,
                orderItemEntity.Quantity,
                orderItemEntity.EstimationPeriod,
                orderItemEntity.ProvisioningType,
                orderItemEntity.CataloguePriceType,
                orderItemEntity.CurrencyCode,
                orderItemEntity.TimeUnit,
                orderItemEntity.PricingUnitTierName,
                orderItemEntity.PricingUnitName,
                orderItemEntity.PricingUnitDescription,
                orderItemEntity.Price,
                orderItemEntity.Created,
                orderItemEntity.LastUpdated);
        }
        
        public OrderItemEntityBuilder WithOrderItemId(int orderItemId)
        {
            _orderItemId = orderItemId;
            return this;
        }

        public OrderItemEntityBuilder WithOrderId(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        public OrderItemEntityBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        public OrderItemEntityBuilder WithCatalogueItemId(string catalogueItemId)
        {
            _catalogueItemId = catalogueItemId;
            return this;
        }

        public OrderItemEntityBuilder WithCatalogueItemType(CatalogueItemType catalogueItemType)
        {
            _catalogueItemType = catalogueItemType;
            return this;
        }

        public OrderItemEntityBuilder WithCatalogueItemName(string catalogueItemName)
        {
            _catalogueItemName = catalogueItemName;
            return this;
        }

        public OrderItemEntityBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate?.Date;
            return this;
        }

        public OrderItemEntityBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        public OrderItemEntityBuilder WithEstimationPeriod(TimeUnit? estimationPeriod)
        {
            _estimationPeriod = estimationPeriod;
            return this;
        }

        public OrderItemEntityBuilder WithProvisioningType(ProvisioningType provisioningType)
        {
            _provisioningType = provisioningType;
            return this;
        }

        public OrderItemEntityBuilder WithCataloguePriceType(CataloguePriceType cataloguePriceType)
        {
            _cataloguePriceType = cataloguePriceType;
            return this;
        }

        public OrderItemEntityBuilder WithCurrencyCode(string currencyCode)
        {
            _currencyCode = currencyCode;
            return this;
        }

        public OrderItemEntityBuilder WithTimeUnit(TimeUnit? timeUnit)
        {
            _timeUnit = timeUnit;
            return this;
        }

        public OrderItemEntityBuilder WithPricingUnitTierName(string pricingUnitTierName)
        {
            _pricingUnitTierName = pricingUnitTierName;
            return this;
        }

        public OrderItemEntityBuilder WithPricingUnitName(string pricingUnitName)
        {
            _pricingUnitName = pricingUnitName;
            return this;
        }

        public OrderItemEntityBuilder WithPricingUnitDescription(string pricingUnitDescription)
        {
            _pricingUnitDescription = pricingUnitDescription;
            return this;
        }

        public OrderItemEntityBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public OrderItemEntityBuilder WithCreated(DateTime created)
        {
            _created = created;
            return this;
        }

        public OrderItemEntity Build()
        {
            return new OrderItemEntity
            {
                OrderItemId = _orderItemId,
                OrderId = _orderId,
                OdsCode = _odsCode,
                CatalogueItemId = _catalogueItemId,
                CatalogueItemType = _catalogueItemType,
                CatalogueItemName = _catalogueItemName,
                DeliveryDate = _deliveryDate,
                Quantity = _quantity,
                EstimationPeriod = _estimationPeriod,
                ProvisioningType = _provisioningType,
                CataloguePriceType = _cataloguePriceType,
                CurrencyCode = _currencyCode,
                TimeUnit = _timeUnit,
                PricingUnitTierName = _pricingUnitTierName,
                PricingUnitName = _pricingUnitName,
                PricingUnitDescription = _pricingUnitDescription,
                Price = _price,
                Created = _created,
                LastUpdated = _lastUpdated
            };
        }
    }
}
