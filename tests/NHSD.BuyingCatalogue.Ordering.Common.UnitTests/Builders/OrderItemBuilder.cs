using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    public sealed class OrderItemBuilder
    {
        private readonly CataloguePriceType cataloguePriceType;
        private readonly List<OrderItemRecipient> recipients = new();

        private CatalogueItem catalogueItem;
        private ProvisioningType provisioningType;

        private PricingUnit pricingUnit;
        private TimeUnit? priceTimeUnit;

        private string currencyCode;
        private TimeUnit? estimationPeriod;
        private DateTime? defaultDeliveryDate;
        private int orderId;
        private decimal? price;

        private OrderItemBuilder()
        {
            catalogueItem = new CatalogueItem { Name = "Doctor Doctor", CatalogueItemType = CatalogueItemType.Solution };
            provisioningType = ProvisioningType.Patient;
            cataloguePriceType = CataloguePriceType.Flat;
            pricingUnit = new PricingUnit { Name = "patient", Description = "per patient" };
            priceTimeUnit = TimeUnit.PerMonth;
            currencyCode = "GBP";
            estimationPeriod = TimeUnit.PerYear;
            defaultDeliveryDate = DateTime.UtcNow;
            orderId = 1;
            price = 2.000m;
        }

        public static OrderItemBuilder Create() => new();

        public OrderItemBuilder WithCatalogueItem(CatalogueItem item)
        {
            catalogueItem = item;
            return this;
        }

        public OrderItemBuilder WithProvisioningType(ProvisioningType type)
        {
            provisioningType = type;
            return this;
        }

        public OrderItemBuilder WithPricingUnit(PricingUnit unit)
        {
            pricingUnit = unit;
            return this;
        }

        public OrderItemBuilder WithPriceTimeUnit(TimeUnit? timeUnit)
        {
            priceTimeUnit = timeUnit;
            return this;
        }

        public OrderItemBuilder WithCurrencyCode(string code)
        {
            currencyCode = code;
            return this;
        }

        public OrderItemBuilder WithEstimationPeriod(TimeUnit? period)
        {
            estimationPeriod = period;
            return this;
        }

        public OrderItemBuilder WithOrderId(int id)
        {
            orderId = id;
            return this;
        }

        public OrderItemBuilder WithRecipient(OrderItemRecipient recipient)
        {
            recipients.Add(recipient);
            return this;
        }

        public OrderItemBuilder WithPrice(decimal? cost)
        {
            price = cost;
            return this;
        }

        public OrderItem Build()
        {
            var orderItem = new OrderItem
            {
                CatalogueItem = catalogueItem,
                CataloguePriceType = cataloguePriceType,
                CurrencyCode = currencyCode,
                DefaultDeliveryDate = defaultDeliveryDate,
                EstimationPeriod = estimationPeriod,
                OrderId = orderId,
                Price = price,
                PricingUnit = pricingUnit,
                PriceTimeUnit = priceTimeUnit,
                ProvisioningType = provisioningType,
            };

            orderItem.SetRecipients(recipients);

            return orderItem;
        }
    }
}
