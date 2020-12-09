using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class ItemUnitModel
    {
        [Required(ErrorMessage = "ItemUnitNameRequired")]
        [MaxLength(20, ErrorMessage = "ItemUnitNameTooLong")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ItemUnitDescriptionRequired")]
        [MaxLength(40, ErrorMessage = "ItemUnitDescriptionTooLong")]
        public string Description { get; set; }
    }
}
