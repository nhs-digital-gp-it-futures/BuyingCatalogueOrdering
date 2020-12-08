using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class UpdateOrderItemModel
    {
        // TODO: consider refactor of model/request structure (item ID is irrelevant for create)
        public int? OrderItemId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string EstimationPeriod { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }
}
