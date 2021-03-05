using FluentValidation;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal sealed class ServiceRecipientModelValidator : AbstractValidator<ServiceRecipientModel>
    {
        public ServiceRecipientModelValidator()
        {
            RuleFor(m => m.OdsCode)
                .Required()
                .MaxLength(8);
        }
    }
}
