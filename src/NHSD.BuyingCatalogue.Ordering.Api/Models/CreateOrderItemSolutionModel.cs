using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemSolutionModel : CreateOrderItemBaseModel
    {
        [Required(ErrorMessage = "CatalogueSolutionIdRequired")]
        [MaxLength(14, ErrorMessage = "CatalogueSolutionIdTooLong")]
        public string CatalogueSolutionId { get; set; }

        [Required(ErrorMessage = "CatalogueSolutionNameRequired")]
        [MaxLength(255, ErrorMessage = "CatalogueSolutionNameTooLong")]
        public string CatalogueSolutionName { get; set; }

        [Required(ErrorMessage = "DeliveryDateRequired")]
        public DateTime? DeliveryDate { get; set; }

        [RequiredWhenProvisioningTypeIn("Declarative", "Patient", ErrorMessage = "TimeUnitRequired")]
        public TimeUnitModel TimeUnit { get; set; }
    }
}
