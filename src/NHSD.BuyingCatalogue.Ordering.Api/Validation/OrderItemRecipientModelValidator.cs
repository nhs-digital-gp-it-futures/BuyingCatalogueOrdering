using FluentValidation;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal sealed class OrderItemRecipientModelValidator : AbstractValidator<OrderItemRecipientModel>
    {
        public OrderItemRecipientModelValidator()
        {
            RuleFor(m => m.Name)
                .MaxLength(256);

            RuleFor(m => m.OdsCode)
                .Required()
                .MaxLength(8);

            RuleFor(m => m.Quantity)
                .Required()
                .GreaterThan(0).WithMessage("{PropertyName}GreaterThanZero")
                .LessThanMax(int.MaxValue);
        }
    }
}
