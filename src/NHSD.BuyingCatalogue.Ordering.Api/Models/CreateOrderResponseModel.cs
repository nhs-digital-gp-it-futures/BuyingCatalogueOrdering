using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class CreateOrderResponseModel
    {
        public string OrderId { get; set; }

        public IEnumerable<ErrorModel> Errors { get; set; }
    }
}
