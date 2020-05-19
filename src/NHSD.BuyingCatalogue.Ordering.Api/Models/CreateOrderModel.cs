using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class CreateOrderModel
    {
        public string Description { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
