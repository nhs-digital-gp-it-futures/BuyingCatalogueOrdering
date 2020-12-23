using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class OrderItemEntityBuilder
    {
        private readonly DateTime lastUpdated;
        private int orderItemId;
        private string orderId;
        private string odsCode;
        private string catalogueItemId;
        private string parentCatalogueItemId;
        private CatalogueItemType catalogueItemType;
        private string catalogueItemName;
        private DateTime? deliveryDate;
        private int quantity;
        private TimeUnit? estimationPeriod;
        private ProvisioningType provisioningType;
        private CataloguePriceType cataloguePriceType;
        private string currencyCode;
        private TimeUnit? timeUnit;
        private string pricingUnitTierName;
        private string pricingUnitName;
        private string pricingUnitDescription;
        private decimal? price;
        private DateTime created;

        private OrderItemEntityBuilder(
            string orderId,
            string odsCode,
            string catalogueItemId,
            string parentCatalogueItemId,
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
            this.orderId = orderId;
            this.odsCode = odsCode;
            this.catalogueItemId = catalogueItemId;
            this.parentCatalogueItemId = parentCatalogueItemId;
            this.catalogueItemType = catalogueItemType;
            this.catalogueItemName = catalogueItemName;
            this.deliveryDate = deliveryDate;
            this.quantity = quantity;
            this.estimationPeriod = estimationPeriod;
            this.provisioningType = provisioningType;
            this.cataloguePriceType = cataloguePriceType;
            this.currencyCode = currencyCode;
            this.timeUnit = timeUnit;
            this.pricingUnitTierName = pricingUnitTierName;
            this.pricingUnitName = pricingUnitName;
            this.pricingUnitDescription = pricingUnitDescription;
            this.price = price;
            this.created = created;
            this.lastUpdated = lastUpdated;
        }

        private OrderItemEntityBuilder() : this(
            "C10000-001",
            "ODS1",
            "100001-001",
            null,
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
                orderItemEntity.ParentCatalogueItemId,
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

        public OrderItemEntityBuilder WithOrderItemId(int id)
        {
            orderItemId = id;
            return this;
        }

        public OrderItemEntityBuilder WithOrderId(string id)
        {
            orderId = id;
            return this;
        }

        public OrderItemEntityBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        public OrderItemEntityBuilder WithCatalogueItemId(string id)
        {
            catalogueItemId = id;
            return this;
        }

        public OrderItemEntityBuilder WithParentCatalogueItemId(string id)
        {
            parentCatalogueItemId = id;
            return this;
        }

        public OrderItemEntityBuilder WithCatalogueItemType(CatalogueItemType type)
        {
            catalogueItemType = type;
            return this;
        }

        public OrderItemEntityBuilder WithCatalogueItemName(string name)
        {
            catalogueItemName = name;
            return this;
        }

        public OrderItemEntityBuilder WithDeliveryDate(DateTime? date)
        {
            deliveryDate = date?.Date;
            return this;
        }

        public OrderItemEntityBuilder WithQuantity(int number)
        {
            quantity = number;
            return this;
        }

        public OrderItemEntityBuilder WithEstimationPeriod(TimeUnit? period)
        {
            estimationPeriod = period;
            return this;
        }

        public OrderItemEntityBuilder WithProvisioningType(ProvisioningType type)
        {
            provisioningType = type;
            return this;
        }

        public OrderItemEntityBuilder WithCataloguePriceType(CataloguePriceType type)
        {
            cataloguePriceType = type;
            return this;
        }

        public OrderItemEntityBuilder WithCurrencyCode(string code)
        {
            currencyCode = code;
            return this;
        }

        public OrderItemEntityBuilder WithTimeUnit(TimeUnit? unit)
        {
            timeUnit = unit;
            return this;
        }

        public OrderItemEntityBuilder WithPricingUnitTierName(string tierName)
        {
            pricingUnitTierName = tierName;
            return this;
        }

        public OrderItemEntityBuilder WithPricingUnitName(string name)
        {
            pricingUnitName = name;
            return this;
        }

        public OrderItemEntityBuilder WithPricingUnitDescription(string description)
        {
            pricingUnitDescription = description;
            return this;
        }

        public OrderItemEntityBuilder WithPrice(decimal? cost)
        {
            price = cost;
            return this;
        }

        public OrderItemEntityBuilder WithCreated(DateTime dateCreated)
        {
            created = dateCreated;
            return this;
        }

        public OrderItemEntity Build()
        {
            return new OrderItemEntity
            {
                OrderItemId = orderItemId,
                OrderId = orderId,
                OdsCode = odsCode,
                CatalogueItemId = catalogueItemId,
                ParentCatalogueItemId = parentCatalogueItemId,
                CatalogueItemType = catalogueItemType,
                CatalogueItemName = catalogueItemName,
                DeliveryDate = deliveryDate,
                Quantity = quantity,
                EstimationPeriod = estimationPeriod,
                ProvisioningType = provisioningType,
                CataloguePriceType = cataloguePriceType,
                CurrencyCode = currencyCode,
                TimeUnit = timeUnit,
                PricingUnitTierName = pricingUnitTierName,
                PricingUnitName = pricingUnitName,
                PricingUnitDescription = pricingUnitDescription,
                Price = price,
                Created = created,
                LastUpdated = lastUpdated,
            };
        }
    }
}
