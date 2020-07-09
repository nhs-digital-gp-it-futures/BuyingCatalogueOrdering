using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class TimeUnitModel
    {
        [Required(ErrorMessage = "TimeUnitNameRequired")]
        [MaxLength(20, ErrorMessage = "TimeUnitNameTooLong")]
        public string Name { get; set; }

        [Required(ErrorMessage = "TimeUnitDescriptionRequired")]
        [MaxLength(32, ErrorMessage = "TimeUnitDescriptionTooLong")]
        public string Description { get; set; }
    }
}
