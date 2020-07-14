using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class UpdateOrderItemRequestBuilder
    {
        private DateTime? _deliveryDate;
        private Order _order;
        private string _estimationPeriodName;
        private int _orderItemId;
        private decimal? _price;
        private int _quantity;

        private UpdateOrderItemRequestBuilder()
        {
            _deliveryDate = DateTime.UtcNow;
            _estimationPeriodName = "year";
            _order = OrderBuilder.Create().Build();
            _orderItemId = 1;
            _price = 2;
            _quantity = 10;
        }

        public static UpdateOrderItemRequestBuilder Create() => new UpdateOrderItemRequestBuilder();

        public UpdateOrderItemRequestBuilder WithOrder(Order order)
        {
            _order = order;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithOrderItemId(int orderItemId)
        {
            _orderItemId = orderItemId;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithEstimationPeriodName(string estimationPeriodName)
        {
            _estimationPeriodName = estimationPeriodName;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate;
            return this;
        }

        public UpdateOrderItemRequestBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public UpdateOrderItemRequest Build() =>
            new UpdateOrderItemRequest(
                _deliveryDate,
                _estimationPeriodName,
                _order,
                _orderItemId,
                _price,
                _quantity
            );
    }
}
