using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class OrderItemEntityBuilder
    {
        private readonly CataloguePriceType cataloguePriceType;

        private int orderId;
        private string catalogueItemId;
        private TimeUnit? estimationPeriod;
        private ProvisioningType provisioningType;
        private string currencyCode;
        private TimeUnit? timeUnit;
        private string pricingUnitName;
        private decimal? price;
        private DateTime created;

        private OrderItemEntityBuilder(
            int orderId,
            string catalogueItemId,
            TimeUnit? estimationPeriod,
            ProvisioningType provisioningType,
            CataloguePriceType cataloguePriceType,
            string currencyCode,
            TimeUnit? timeUnit,
            string pricingUnitName,
            decimal? price,
            DateTime created)
        {
            this.orderId = orderId;
            this.catalogueItemId = catalogueItemId;
            this.estimationPeriod = estimationPeriod;
            this.provisioningType = provisioningType;
            this.cataloguePriceType = cataloguePriceType;
            this.currencyCode = currencyCode;
            this.timeUnit = timeUnit;
            this.pricingUnitName = pricingUnitName;
            this.price = price;
            this.created = created;
        }

        private OrderItemEntityBuilder()
            : this(
                10001,
                "10001-001",
                TimeUnit.Month,
                ProvisioningType.OnDemand,
                CataloguePriceType.Flat,
                "GBP",
                null,
                "patient",
                1.5m,
                DateTime.UtcNow)
        {
        }

        public static OrderItemEntityBuilder Create() => new();

        public static OrderItemEntityBuilder Create(OrderItemEntity orderItemEntity)
        {
            if (orderItemEntity is null)
                throw new ArgumentNullException(nameof(orderItemEntity));

            return new OrderItemEntityBuilder(
                orderItemEntity.OrderId,
                orderItemEntity.CatalogueItemId,
                orderItemEntity.EstimationPeriod,
                orderItemEntity.ProvisioningType,
                orderItemEntity.CataloguePriceType,
                orderItemEntity.CurrencyCode,
                orderItemEntity.TimeUnit,
                orderItemEntity.PricingUnitName,
                orderItemEntity.Price,
                orderItemEntity.Created);
        }

        public OrderItemEntityBuilder WithOrderId(int id)
        {
            orderId = id;
            return this;
        }

        public OrderItemEntityBuilder WithOdsCode(string code)
        {
            _ = code;
            return this;
        }

        public OrderItemEntityBuilder WithCatalogueItemId(string id)
        {
            catalogueItemId = id;
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

        public OrderItemEntityBuilder WithPricingUnitName(string name)
        {
            pricingUnitName = name;
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
            return new()
            {
                OrderId = orderId,
                CatalogueItemId = catalogueItemId,
                EstimationPeriod = estimationPeriod,
                ProvisioningType = provisioningType,
                CataloguePriceType = cataloguePriceType,
                CurrencyCode = currencyCode,
                TimeUnit = timeUnit,
                PricingUnitName = pricingUnitName,
                Price = price,
                Created = created,
                LastUpdated = created,
            };
        }
    }
}
