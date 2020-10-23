using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class UpdateOrderItemRequestBuilder
    {
        private DateTime? deliveryDate;
        private Order order;
        private string estimationPeriodName;
        private int orderItemId;
        private decimal? price;
        private int quantity;

        private UpdateOrderItemRequestBuilder()
        {
            deliveryDate = DateTime.UtcNow;
            estimationPeriodName = "year";
            order = OrderBuilder.Create().Build();
            orderItemId = 1;
            price = 2;
            quantity = 10;
        }

        public static UpdateOrderItemRequestBuilder Create() => new UpdateOrderItemRequestBuilder();

        public UpdateOrderItemRequestBuilder WithOrder(Order o)
        {
            order = o;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithOrderItemId(int itemId)
        {
            orderItemId = itemId;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithQuantity(int number)
        {
            quantity = number;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithEstimationPeriodName(string name)
        {
            estimationPeriodName = name;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithDeliveryDate(DateTime? date)
        {
            this.deliveryDate = date;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithPrice(decimal? cost)
        {
            price = cost;
            return this;
        }

        public UpdateOrderItemRequest Build() =>
            new UpdateOrderItemRequest(
                deliveryDate,
                estimationPeriodName,
                order,
                orderItemId,
                price,
                quantity
            );
    }
}
