using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Attributes
{
    public sealed class CommencementDateAttribute : ValidationAttribute
    {
        public CommencementDateAttribute(int days = -60)
        {
            Days = days;
        }

        public int Days { get; }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (validationContext is null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            if (value is null)
            {
                return ValidationResult.Success;
            }

            var commencementDate = (DateTime)value;

            return commencementDate.ToUniversalTime() <= DateTime.UtcNow.AddDays(Days)
                ? new ValidationResult(ErrorMessage)
                : ValidationResult.Success;
        }
    }
}
