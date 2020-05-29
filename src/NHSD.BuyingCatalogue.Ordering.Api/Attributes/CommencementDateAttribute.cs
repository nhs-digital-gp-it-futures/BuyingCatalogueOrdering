using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Attributes
{

    public class CommencementDateAttribute : ValidationAttribute
    {
        private int _days;
        public CommencementDateAttribute(int days = 60)
        {
            _days = days;
        }

        protected override ValidationResult IsValid(object value,
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

            if (commencementDate.ToUniversalTime() <= DateTime.UtcNow.AddDays(-60))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
