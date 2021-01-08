using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem
{
    public class UpdateOrderItemRequest
    {
        public UpdateOrderItemRequest(Order order, UpdateOrderItemModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            // Model validation should ensure Quantity and Price are not null under normal circumstances
            if (model.Quantity is null)
                throw ModelPropertyIsNullException(nameof(model), nameof(UpdateOrderItemModel.Quantity));

            if (model.Price is null)
                throw ModelPropertyIsNullException(nameof(model), nameof(UpdateOrderItemModel.Price));

            DeliveryDate = model.DeliveryDate;
            EstimationPeriod = OrderingEnums.ParseTimeUnit(model.EstimationPeriod);
            Order = order ?? throw new ArgumentNullException(nameof(order));
            OrderItemId = model.OrderItemId.GetValueOrDefault();
            Quantity = model.Quantity.Value;
            Price = model.Price.Value;
        }

        public DateTime? DeliveryDate { get; protected set; }

        public TimeUnit? EstimationPeriod { get; }

        public Order Order { get; }

        public int OrderItemId { get; set; }

        public decimal Price { get; }

        public int Quantity { get; }

        private static ArgumentException ModelPropertyIsNullException(string modelParamName, string propertyName)
        {
            return new($"{modelParamName}.{propertyName} should not be null", modelParamName);
        }
    }
}
