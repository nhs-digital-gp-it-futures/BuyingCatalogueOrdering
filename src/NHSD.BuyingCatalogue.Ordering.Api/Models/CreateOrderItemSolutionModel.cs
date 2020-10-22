using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemSolutionModel : CreateOrderItemModel
    {
        [Required(ErrorMessage = "DeliveryDateRequired")]
        public DateTime? DeliveryDate { get; set; }

        [RequiredWhenProvisioningTypeIn("Declarative", "Patient", ErrorMessage = "TimeUnitRequired")]
        public TimeUnitModel TimeUnit { get; set; }

        [Required(ErrorMessage = "ServiceRecipientRequired")]
        public ServiceRecipientModel ServiceRecipient { get; set; }

        internal override ServiceRecipientModel ServiceRecipientModel => ServiceRecipient;

        protected override TimeUnitModel TimeUnitModel => TimeUnit;

        protected override DateTime? GetItemDeliveryDate() => DeliveryDate;

        protected override CatalogueItemType GetItemType() => Domain.CatalogueItemType.Solution;

        protected override string GetOdsCode(Order order) => ServiceRecipient?.OdsCode;
    }
}
