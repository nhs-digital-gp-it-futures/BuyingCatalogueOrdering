using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderModel
    {
        public string Description { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
