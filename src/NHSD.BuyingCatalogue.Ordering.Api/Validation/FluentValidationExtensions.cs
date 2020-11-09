using System;
using FluentValidation;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal static class FluentValidationExtensions
    {
        internal static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder, string message = null)
        {
            return ruleBuilder.Cascade(CascadeMode.Stop).NotEmpty().WithMessage(message ?? "{PropertyName}Required");
        }

        internal static IRuleBuilderOptions<T, TProperty?> LessThanMax<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder.LessThan(valueToCompare).WithLessThanMaxMessage();
        }

        internal static IRuleBuilderOptions<T, TProperty?> LessThanOrEqualToMax<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder.LessThanOrEqualTo(valueToCompare).WithLessThanMaxMessage();
        }

        internal static IRuleBuilderOptions<T, string> MaxLength<T>(this IRuleBuilder<T, string> ruleBuilder, int maximumLength)
        {
            return ruleBuilder.MaximumLength(maximumLength).WithMessage("{PropertyName}TooLong");
        }

        internal static IRuleBuilderOptions<T, string> IsEnumName<T, TEnum>(this IRuleBuilder<T, string> ruleBuilder)
            where TEnum : struct, Enum
        {
            return ruleBuilder.IsEnumName(typeof(TEnum), false).WithValidValueMessage();
        }

        internal static IRuleBuilderOptions<T, TProperty> WithLessThanMaxMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage("{PropertyName}LessThanMax");
        }

        internal static IRuleBuilderOptions<T, TProperty> WithValidValueMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage("{PropertyName}ValidValue");
        }
    }
}
