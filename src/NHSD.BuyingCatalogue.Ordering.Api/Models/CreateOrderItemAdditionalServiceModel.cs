using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemAdditionalServiceModel : CreateOrderItemModel
    {
        [Required(ErrorMessage = "CatalogueSolutionIdRequired")]
        [MaxLength(14, ErrorMessage = "CatalogueSolutionIdTooLong")]
        public string CatalogueSolutionId { get; set; }

        [RequiredWhenProvisioningTypeIn("Declarative", "Patient", ErrorMessage = "TimeUnitRequired")]
        public TimeUnitModel TimeUnit { get; set; }

        [Required(ErrorMessage = "ServiceRecipientRequired")]
        public ServiceRecipientModel ServiceRecipient { get; set; }

        protected override TimeUnitModel TimeUnitModel => TimeUnit;

        protected override string GetAssociatedCatalogueItemId() => CatalogueSolutionId;

        protected override CatalogueItemType GetItemType() => Domain.CatalogueItemType.AdditionalService;

        protected override string GetOdsCode(Order order) => ServiceRecipient?.OdsCode;
    }
}
