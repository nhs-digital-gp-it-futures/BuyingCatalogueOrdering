using System;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class UpdateCatalogueSolutionOrderItemRequestPayloadBuilder
    {
        private DateTime? _deliveryDate;
        private TimeUnit? _estimationPeriod;
        private decimal? _price;
        private int? _quantity;

        private UpdateCatalogueSolutionOrderItemRequestPayloadBuilder()
        {
            _deliveryDate = new DateTime(2021, 1, 1);
            _estimationPeriod = TimeUnit.Month;
            _price = 100.0m;
            _quantity = 100;
        }

        public static UpdateCatalogueSolutionOrderItemRequestPayloadBuilder Create() => 
            new UpdateCatalogueSolutionOrderItemRequestPayloadBuilder();

        public UpdateCatalogueSolutionOrderItemRequestPayloadBuilder WithDeliveryDate(DateTime? deliveryDate)
        {
            _deliveryDate = deliveryDate;
            return this;
        }

        public UpdateCatalogueSolutionOrderItemRequestPayloadBuilder WithEstimationPeriod(TimeUnit? estimationPeriod)
        {
            _estimationPeriod = estimationPeriod;
            return this;
        }

        public UpdateCatalogueSolutionOrderItemRequestPayloadBuilder WithPrice(decimal? price)
        {
            _price = price;
            return this;
        }

        public UpdateCatalogueSolutionOrderItemRequestPayloadBuilder WithQuantity(int? quantity)
        {
            _quantity = quantity;
            return this;
        }

        public UpdateCatalogueSolutionOrderItemRequestPayload Build()
        {
            return new UpdateCatalogueSolutionOrderItemRequestPayload
            {
                DeliveryDate = _deliveryDate,
                EstimationPeriod = _estimationPeriod,
                Price = _price,
                Quantity = _quantity,
            };
        }
    }
}
