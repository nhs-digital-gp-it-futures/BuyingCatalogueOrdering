using FluentValidation;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal sealed class OrderDescriptionModelValidator : AbstractValidator<OrderDescriptionModel>
    {
        public OrderDescriptionModelValidator()
        {
            RuleFor(m => m.Description)
                .Required()
                .MaxLength(100);
        }
    }
}
