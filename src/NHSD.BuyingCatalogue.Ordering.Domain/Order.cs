using System;


namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Order
    {
        public string OrderId { get; set; }
         
        public OrderDescription Description { get; private set; }

        public Guid OrganisationId { get; set; }

        public DateTime Created { get; set; }
         
        public DateTime LastUpdated { get; set; }
        
        public Guid LastUpdatedBy { get; set; }
                

        public OrderStatus OrderStatus { get; set; }

        public void SetDescription(OrderDescription orderDescription)
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
        }
    }
}
