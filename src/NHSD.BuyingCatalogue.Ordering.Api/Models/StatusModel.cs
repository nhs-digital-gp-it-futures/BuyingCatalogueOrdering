using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class StatusModel
    {
        [Required(ErrorMessage = "StatusRequired")]
        public string Status { get; set; }
    }
}
