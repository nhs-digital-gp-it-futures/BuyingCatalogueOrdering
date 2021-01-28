using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class UpdateOrderItemModelBuilder
    {
        private readonly DateTime? deliveryDate;
        private readonly string estimationPeriod;
        private readonly decimal? price;
        private readonly int? quantity = 123;

        private UpdateOrderItemModelBuilder()
        {
            deliveryDate = DateTime.UtcNow;
            estimationPeriod = "month";
            price = 25.1m;
        }

        public static UpdateOrderItemModelBuilder Create() => new();

        public UpdateOrderItemModel BuildSolution() => new()
        {
            DeliveryDate = deliveryDate,
            Quantity = quantity,
            EstimationPeriod = estimationPeriod,
            Price = price,
        };
    }
}
