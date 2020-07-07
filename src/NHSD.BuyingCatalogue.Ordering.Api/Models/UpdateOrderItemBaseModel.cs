using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public abstract class UpdateOrderItemBaseModel
    {
        [Required(ErrorMessage = "QuantityRequired")]
        [Limit(1, LimitType.Minimum, ErrorMessage = "QuantityGreaterThanZero")]
        [Limit(int.MaxValue - 1, LimitType.Maximum, ErrorMessage = "QuantityLessThanMax")]
        public int? Quantity { get; set; }

        public string EstimationPeriod { get; set; }

        [Required(ErrorMessage = "PriceRequired")]
        [Limit(0d, LimitType.Minimum, ErrorMessage = "PriceGreaterThanOrEqualToZero")]
        [Limit(typeof(decimal), "999999999999999.999", LimitType.Maximum, ErrorMessage = "PriceLessThanMax")]
        public decimal? Price { get; set; }
    }
}
