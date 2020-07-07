using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class UpdateOrderItemSolutionModel : UpdateOrderItemBaseModel
    {
        [Required(ErrorMessage = "DeliveryDateRequired")]
        public DateTime? DeliveryDate { get; set; }
    }
}
