using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
