using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    public class CreateOrderRequest
    {
        public Guid LastUpdatedById { get; set; }
        public string LastUpdatedByName { get; set; }

        public string Description { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
