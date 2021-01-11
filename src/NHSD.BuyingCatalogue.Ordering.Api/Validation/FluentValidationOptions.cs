using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal static class FluentValidationOptions
    {
        internal static string DisplayNameResolver(Type type, MemberInfo member, LambdaExpression expression)
        {
            _ = type;
            _ = expression;

            return member?.Name;
        }
    }
}
