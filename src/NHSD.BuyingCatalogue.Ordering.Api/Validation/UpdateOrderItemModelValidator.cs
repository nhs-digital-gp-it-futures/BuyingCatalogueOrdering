using FluentValidation;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal class UpdateOrderItemModelValidator<T> : AbstractValidator<T>
        where T : UpdateOrderItemModel
    {
        internal const decimal MaxPrice = 999999999999999.999m;

        public UpdateOrderItemModelValidator()
        {
            RuleFor(m => m.Quantity)
                .Required()
                .GreaterThan(0).WithMessage("{PropertyName}GreaterThanZero")
                .LessThanMax(int.MaxValue);

            RuleFor(m => m.Price)
                .Required()
                .GreaterThanOrEqualTo(0.00m).WithMessage("{PropertyName}GreaterThanOrEqualToZero")
                .LessThanOrEqualToMax(MaxPrice);
        }
    }
}
