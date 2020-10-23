using System;
using System.Reflection;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    public sealed class OrderItemBuilder
    {
        private int? orderItemId;
        private string odsCode;
        private string catalogueItemId;
        private CatalogueItemType catalogueItemType;
        private string catalogueItemName;
        private ProvisioningType provisioningType;
        private CataloguePriceType cataloguePriceType;
        private CataloguePriceUnit cataloguePriceUnit;
        private TimeUnit? priceTimeUnit;
        private string currencyCode;
        private int quantity;
        private TimeUnit? estimationPeriod;
        private DateTime? deliveryDate;
        private decimal? price;
        private DateTime created;

        private OrderItemBuilder()
        {
            odsCode = "ODS1";
            catalogueItemId = "1000-001";
            catalogueItemType = CatalogueItemType.Solution;
            catalogueItemName = Guid.NewGuid().ToString();
            provisioningType = ProvisioningType.Patient;
            cataloguePriceType = CataloguePriceType.Flat;
            cataloguePriceUnit = CataloguePriceUnit.Create("patients", "per patient");
            priceTimeUnit = TimeUnit.PerMonth;
            currencyCode = "GBP";
            quantity = 10;
            estimationPeriod = TimeUnit.PerYear;
            deliveryDate = DateTime.UtcNow;
            price = 2.000m;
            created = DateTime.UtcNow;
        }

        public static OrderItemBuilder Create() =>
            new OrderItemBuilder();

        public OrderItemBuilder WithOrderItemId(int id)
        {
            orderItemId = id;
            return this;
        }

        public OrderItemBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        public OrderItemBuilder WithCatalogueItemId(string id)
        {
            catalogueItemId = id;
            return this;
        }

        public OrderItemBuilder WithCatalogueItemType(CatalogueItemType itemType)
        {
            catalogueItemType = itemType;
            return this;
        }

        public OrderItemBuilder WithCatalogueItemName(string name)
        {
            catalogueItemName = name;
            return this;
        }

        public OrderItemBuilder WithProvisioningType(ProvisioningType type)
        {
            provisioningType = type;
            return this;
        }

        public OrderItemBuilder WithCataloguePriceType(CataloguePriceType priceType)
        {
            cataloguePriceType = priceType;
            return this;
        }

        public OrderItemBuilder WithCataloguePriceUnit(CataloguePriceUnit priceUnit)
        {
            cataloguePriceUnit = priceUnit;
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

        public OrderItemBuilder WithQuantity(int number)
        {
            quantity = number;
            return this;
        }

        public OrderItemBuilder WithEstimationPeriod(TimeUnit? period)
        {
            estimationPeriod = period;
            return this;
        }

        public OrderItemBuilder WithDeliveryDate(DateTime? date)
        {
            deliveryDate = date;
            return this;
        }

        public OrderItemBuilder WithPrice(decimal? cost)
        {
            price = cost;
            return this;
        }

        public OrderItemBuilder WithCreated(DateTime dateCreated)
        {
            created = dateCreated;
            return this;
        }

        public OrderItem Build()
        {
            var orderItem = new OrderItem(
                odsCode,
                catalogueItemId,
                catalogueItemType,
                catalogueItemName,
                null,
                provisioningType,
                cataloguePriceType,
                cataloguePriceUnit,
                priceTimeUnit,
                currencyCode,
                quantity,
                estimationPeriod,
                deliveryDate,
                price);

            if (orderItemId.HasValue)
            {
                var fieldInfo = orderItem.GetType().GetField("_orderItemId", BindingFlags.Instance | BindingFlags.NonPublic);
                fieldInfo?.SetValue(orderItem, orderItemId.Value);
            }

            var createdFieldInfo = orderItem.GetType().GetField("_created", BindingFlags.Instance | BindingFlags.NonPublic);
            createdFieldInfo?.SetValue(orderItem, created);

            return orderItem;
        }
    }
}
