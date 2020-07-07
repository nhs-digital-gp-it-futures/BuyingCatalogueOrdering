using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RequiredWhenProvisioningTypeInAttribute : ValidationAttribute
    {
        private readonly IEnumerable<string> _provisioningTypes;

        public RequiredWhenProvisioningTypeInAttribute(params string[] provisioningTypes)
        {
            _provisioningTypes = provisioningTypes ?? throw new ArgumentNullException(nameof(provisioningTypes));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null) throw new ArgumentNullException(nameof(validationContext));

            if (!(validationContext.ObjectInstance is CreateOrderItemBaseModel orderItem))
            {
                throw new ArgumentException("The OrderItemRequired attribute should only be applied to CreateOrderItemModels");
            }

            if (!_provisioningTypes.Any(s => s.Equals(orderItem.ProvisioningType, StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
