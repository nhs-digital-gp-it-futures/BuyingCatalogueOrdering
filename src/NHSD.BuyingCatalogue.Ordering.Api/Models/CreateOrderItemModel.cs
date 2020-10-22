using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class CreateOrderItemModel : CreateOrderItemBaseModel
    {
        [Required(ErrorMessage = "CatalogueItemIdRequired")]
        [MaxLength(14, ErrorMessage = "CatalogueItemIdTooLong")]
        public string CatalogueItemId { get; set; }

        [Required(ErrorMessage = "CatalogueItemNameRequired")]
        [MaxLength(255, ErrorMessage = "CatalogueItemNameTooLong")]
        public string CatalogueItemName { get; set; }

        [Required(ErrorMessage = "ProvisioningTypeRequired")]
        public string ProvisioningType { get; set; }

        [Required(ErrorMessage = "TypeRequired")]
        public string Type { get; set; }

        [Required(ErrorMessage = "CurrencyCodeRequired")]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = "ItemUnitRequired")]
        public ItemUnitModel ItemUnit { get; set; }

        [Required(ErrorMessage = "QuantityRequired")]
        [Limit(1, LimitType.Minimum, ErrorMessage = "QuantityGreaterThanZero")]
        [Limit(int.MaxValue - 1, LimitType.Maximum, ErrorMessage = "QuantityLessThanMax")]
        public int? Quantity { get; set; }

        public string EstimationPeriod { get; set; }

        [Required(ErrorMessage = "PriceRequired")]
        [Limit(typeof(decimal), "0", LimitType.Minimum, ErrorMessage = "PriceGreaterThanOrEqualToZero")]
        [Limit(typeof(decimal), "999999999999999.999", LimitType.Maximum, ErrorMessage = "PriceLessThanMax")]
        public decimal? Price { get; set; }

        public override CreateOrderItemRequest ToRequest(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (Quantity is null)
                throw new InvalidOperationException($"Model {nameof(Quantity)} should never be null at this point");

            return new CreateOrderItemRequest(
                order,
                GetOdsCode(order),
                CatalogueItemId,
                GetItemType(),
                CatalogueItemName,
                GetAssociatedCatalogueItemId(),
                ProvisioningType,
                Type,
                ItemUnit?.Name,
                ItemUnit?.Description,
                TimeUnitModelToTimeUnit(),
                CurrencyCode,
                Quantity.Value,
                EstimationPeriod,
                GetItemDeliveryDate(),
                Price);
        }
    }
}
