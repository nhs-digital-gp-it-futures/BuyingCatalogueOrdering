using System;

namespace NHSD.BuyingCatalogue.Ordering.Application.Services.CreateOrder
{
    public sealed class CreateOrderRequest
    {
        public Guid LastUpdatedById { get; set; }

        public string LastUpdatedByName { get; set; }

        public string Description { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
