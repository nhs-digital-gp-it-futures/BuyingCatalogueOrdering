using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class UpdateOrderItemModel
    {
        public DateTime? DeliveryDate { get; set; }

        public string EstimationPeriod { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }
}
