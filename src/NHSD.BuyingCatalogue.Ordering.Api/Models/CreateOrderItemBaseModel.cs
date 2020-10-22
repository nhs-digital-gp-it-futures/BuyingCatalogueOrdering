using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    [JsonConverter(typeof(CreateOrderItemBaseModelConverter))]
    public abstract class CreateOrderItemBaseModel
    {
        [Required(ErrorMessage = "CatalogueItemTypeRequired")]
        [RegularExpression("Solution|AdditionalService|AssociatedService", ErrorMessage = "CatalogueItemTypeValidValue")]
        public string CatalogueItemType { get; set; }

        protected virtual TimeUnitModel TimeUnitModel => null;

        public abstract CreateOrderItemRequest ToRequest(Order order);

        internal virtual ServiceRecipientModel ServiceRecipientModel => null;

        protected virtual string GetAssociatedCatalogueItemId() => null;

        protected virtual CatalogueItemType GetItemType() => Domain.CatalogueItemType.Solution;

        protected virtual string GetOdsCode(Order order) => null;

        protected virtual TimeUnit? TimeUnitModelToTimeUnit() => TimeUnitModel?.ToTimeUnit();

        protected virtual DateTime? GetItemDeliveryDate() => null;
    }
}
