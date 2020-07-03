using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public abstract class UpdateOrderItemBaseModel
    {
        [Required(ErrorMessage = "QuantityRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "QuantityGreaterThanZero")]
        public int? Quantity { get; set; }

        public string EstimationPeriod { get; set; }

        [Required(ErrorMessage = "PriceRequired")]
        [Range(0, double.MaxValue, ErrorMessage = "QuantityGreaterThanZero")]
        public decimal? Price { get; set; }
    }
}
