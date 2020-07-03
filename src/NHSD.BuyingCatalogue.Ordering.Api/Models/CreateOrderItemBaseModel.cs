using System.ComponentModel.DataAnnotations;

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
        [Range(1, int.MaxValue, ErrorMessage = "QuantityGreaterThanZero")]
        public int? Quantity { get; set; }

        public string EstimationPeriod { get; set; }

        [Required(ErrorMessage = "PriceRequired")]
        [Range(0, double.MaxValue, ErrorMessage = "QuantityGreaterThanZero")]
        public decimal? Price { get; set; }
    }
}
