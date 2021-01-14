using System;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class UpdateOrderItemRequestPayloadBuilder
    {
        private DateTime? deliveryDate;
        private TimeUnit? estimationPeriod;
        private decimal? price;
        private int? quantity;

        private UpdateOrderItemRequestPayloadBuilder()
        {
            deliveryDate = new DateTime(2021, 1, 1);
            estimationPeriod = TimeUnit.Month;
            price = 100.0m;
            quantity = 100;
        }

        public static UpdateOrderItemRequestPayloadBuilder Create() => new();

        public UpdateOrderItemRequestPayloadBuilder WithDeliveryDate(DateTime? date)
        {
            deliveryDate = date;
            return this;
        }

        public UpdateOrderItemRequestPayloadBuilder WithEstimationPeriod(TimeUnit? period)
        {
            estimationPeriod = period;
            return this;
        }

        public UpdateOrderItemRequestPayloadBuilder WithPrice(decimal? cost)
        {
            price = cost;
            return this;
        }

        public UpdateOrderItemRequestPayloadBuilder WithQuantity(int? number)
        {
            quantity = number;
            return this;
        }

        public UpdateOrderItemRequestPayload Build()
        {
            return new()
            {
                DeliveryDate = deliveryDate,
                EstimationPeriod = estimationPeriod,
                Price = price,
                Quantity = quantity,
            };
        }
    }
}
