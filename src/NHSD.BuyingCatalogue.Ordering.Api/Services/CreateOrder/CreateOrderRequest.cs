using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    public class CreateOrderRequest
    {
        public string Description { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
