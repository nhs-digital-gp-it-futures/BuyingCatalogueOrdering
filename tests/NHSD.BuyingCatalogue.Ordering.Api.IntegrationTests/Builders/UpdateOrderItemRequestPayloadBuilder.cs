using System;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class UpdateOrderItemRequestPayloadBuilder
    {
        private DateTime? _deliveryDate;
        private TimeUnit? _estimationPeriod;
        private decimal? _price;
        private int? _quantity;

        private UpdateOrderItemRequestPayloadBuilder()
        {
            _deliveryDate = new DateTime(2021, 1, 1);
            _estimationPeriod = TimeUnit.Month;
            _price = 100.0m;
            _quantity = 100;
        }

        public static UpdateOrderItemRequestPayloadBuilder Create() => new();

        public UpdateOrderItemRequestPayloadBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate;
            return this;
        }

        public UpdateOrderItemRequestPayloadBuilder WithEstimationPeriod(TimeUnit? estimationPeriod)
        {
            _estimationPeriod = estimationPeriod;
            return this;
        }

        public UpdateOrderItemRequestPayloadBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public UpdateOrderItemRequestPayloadBuilder WithQuantity(int? quantity)
        {
            _quantity = quantity;
            return this;
        }

        public UpdateOrderItemRequestPayload Build()
        {
            return new()
            {
                DeliveryDate = _deliveryDate,
                EstimationPeriod = _estimationPeriod,
                Price = _price,
                Quantity = _quantity,
            };
        }
    }
}
