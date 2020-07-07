using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public abstract class CreateOrderItemBaseModel
    {
        [Required(ErrorMessage = "ServiceRecipientRequired")]
        public ServiceRecipientModel ServiceRecipient { get; set; }

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
    }
}
