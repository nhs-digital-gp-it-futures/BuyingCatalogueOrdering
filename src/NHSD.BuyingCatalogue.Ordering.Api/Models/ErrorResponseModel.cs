using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class ErrorResponseModel
    {
        public string OrderId { get; set; }

        public IEnumerable<ErrorModel> Errors { get; set; }
    }
}
