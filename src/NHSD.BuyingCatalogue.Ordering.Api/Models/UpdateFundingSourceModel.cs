using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class UpdateFundingSourceModel
    {
        [Required(ErrorMessage = "OnlyGMSRequired")]
        public bool? OnlyGms { get; set; }
    }
}
