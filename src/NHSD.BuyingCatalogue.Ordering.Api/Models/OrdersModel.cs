using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrdersModel
    {
        public Guid OrderId { get; set; }

        public string OrderDescription { get; set; }

        public string LastUpdatedBy { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        public string Status { get; set; }
    }
}
