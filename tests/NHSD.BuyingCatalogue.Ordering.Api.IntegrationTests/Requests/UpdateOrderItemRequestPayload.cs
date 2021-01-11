using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class UpdateOrderItemRequestPayload
    {
        public DateTime? DeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public decimal? Price { get; set; }
    }
}