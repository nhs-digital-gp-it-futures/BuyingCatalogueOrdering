using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CommencementDateModel
    {
        [Required(ErrorMessage = "CommencementDateRequired")]
        [CommencementDate(ErrorMessage = "CommencementDateGreaterThan")]
        public DateTime? CommencementDate { get; set; }
    }
}
