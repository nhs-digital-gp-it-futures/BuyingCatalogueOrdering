using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class CreateOrderResponseModel
    {
        public string OrderId { get; set; }

        public IEnumerable<ErrorMessageModel> Errors { get; set; }
    }
}
