using System;
using FluentValidation;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal sealed class CreateOrderModelValidator : AbstractValidator<CreateOrderModel>
    {
        public CreateOrderModelValidator()
        {
            RuleFor(m => m.Description)
                .Required()
                .MaxLength(100);

            RuleFor(m => m.OrganisationId)
                .Must(i => i != Guid.Empty)
                .WithRequiredMessage();
        }
    }
}
