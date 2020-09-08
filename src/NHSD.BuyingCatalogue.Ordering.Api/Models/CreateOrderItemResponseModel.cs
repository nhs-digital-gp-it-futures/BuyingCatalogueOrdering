using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemResponseModel
    {
        public int? OrderItemId { get; set; }

        public IEnumerable<ErrorModel> Errors { get; set; }
    }
}
