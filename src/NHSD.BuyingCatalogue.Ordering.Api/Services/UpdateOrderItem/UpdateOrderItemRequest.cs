using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem
{
    public sealed class UpdateOrderItemRequest
    {
        public UpdateOrderItemRequest(
            DateTime? deliveryDate,
            string estimationPeriodName,
            Order order,
            int orderItemId,
            decimal? price,
            int? quantity)
        {
            DeliveryDate = deliveryDate;
            EstimationPeriod = OrderingEnums.ParseTimeUnit(estimationPeriodName);
            Order = order ?? throw new ArgumentNullException(nameof(order));
            OrderItemId = orderItemId;
            Quantity = quantity.GetValueOrDefault();
            Price = price;
        }

        public DateTime? DeliveryDate { get; }

        public TimeUnit? EstimationPeriod { get; }

        public Order Order { get; }

        public int OrderItemId { get; set; }

        public decimal? Price { get; }

        public int Quantity { get; }
    }
}
