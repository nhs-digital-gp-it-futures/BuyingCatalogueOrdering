using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class StatusModel
    {
        [Required(ErrorMessage = "StatusRequired")]
        [RegularExpression("Complete|Incomplete", ErrorMessage = "InvalidStatus")]
        public string Status { get; set; }
    }
}
