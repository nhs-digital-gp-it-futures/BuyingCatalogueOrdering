using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class OrderItemRequiredAttribute : ValidationAttribute
    {
        private readonly IEnumerable<string> _provisioningTypes;

        public OrderItemRequiredAttribute(params string[] provisioningProvisioningTypes)
        {
            _provisioningTypes = provisioningProvisioningTypes ?? throw new ArgumentNullException(nameof(provisioningProvisioningTypes));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null) throw new ArgumentNullException(nameof(validationContext));

            if (!(validationContext.ObjectInstance is CreateOrderItemBaseModel orderItem))
            {
                throw new ArgumentException("The OrderItemRequired attribute should only be applied to CreateOrderItemModels");
            }

            if (!_provisioningTypes.Contains(orderItem.ProvisioningType))
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
