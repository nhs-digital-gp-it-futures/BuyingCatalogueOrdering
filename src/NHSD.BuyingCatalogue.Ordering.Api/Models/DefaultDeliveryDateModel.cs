using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class DefaultDeliveryDateModel
    {
        [Required(ErrorMessage = nameof(DeliveryDate) + "Required")]
        public DateTime? DeliveryDate { get; set; }
    }
}
