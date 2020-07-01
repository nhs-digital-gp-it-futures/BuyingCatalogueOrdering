using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class UpdateOrderItemResponseModel
    {
        public IEnumerable<ErrorModel> Errors { get; set; }
    }
}
