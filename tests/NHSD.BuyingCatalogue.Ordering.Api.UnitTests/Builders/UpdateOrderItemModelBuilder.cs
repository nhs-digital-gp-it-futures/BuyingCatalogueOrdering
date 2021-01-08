using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class UpdateOrderItemModelBuilder
    {
        private readonly DateTime? _deliveryDate;
        private int? _quantity;
        private readonly string _estimationPeriod;
        private readonly decimal? _price;

        private UpdateOrderItemModelBuilder()
        {
            _deliveryDate = DateTime.UtcNow;
            _quantity = 123;
            _estimationPeriod = "month";
            _price = 25.1m;
        }

        public UpdateOrderItemModelBuilder WithQuantity(int? quantity)
        {
            _quantity = quantity;
            return this;
        }

        public static UpdateOrderItemModelBuilder Create() => new();

        public UpdateOrderItemModel BuildSolution() => new()
            {
                DeliveryDate = _deliveryDate,
                Quantity = _quantity,
                EstimationPeriod = _estimationPeriod,
                Price = _price,
            };
    }
}
