using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderModel
    {
        public string Description { get; init; }

        public Guid? OrganisationId { get; init; }
    }
}
