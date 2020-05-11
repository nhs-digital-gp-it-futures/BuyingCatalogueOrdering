using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Order
    {
        public string OrderId { get; set; }
         
        public string Description { get; set; }

        public Guid OrganisationId { get; set; }

        public DateTime Created { get; set; }
         
        public DateTime LastUpdated { get; set; }
        
        public Guid LastUpdatedBy { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}
