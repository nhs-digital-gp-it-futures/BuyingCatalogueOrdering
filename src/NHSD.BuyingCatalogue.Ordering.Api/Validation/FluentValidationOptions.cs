using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    internal static class FluentValidationOptions
    {
        internal static string DisplayNameResolver(Type _, MemberInfo member, LambdaExpression _1) =>
            member?.Name;
    }
}
