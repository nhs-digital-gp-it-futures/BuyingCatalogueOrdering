using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class UpdateOrderItemModelBuilder
    {
        private readonly DateTime? deliveryDate;
        private readonly string estimationPeriod;
        private readonly decimal? price;

        private int? quantity;

        private UpdateOrderItemModelBuilder()
        {
            deliveryDate = DateTime.UtcNow;
            quantity = 123;
            estimationPeriod = "month";
            price = 25.1m;
        }

        public static UpdateOrderItemModelBuilder Create() => new();

        public UpdateOrderItemModelBuilder WithQuantity(int? number)
        {
            quantity = number;
            return this;
        }

        public UpdateOrderItemModel BuildSolution() => new()
        {
            DeliveryDate = deliveryDate,
            Quantity = quantity,
            EstimationPeriod = estimationPeriod,
            Price = price,
        };
    }
}
