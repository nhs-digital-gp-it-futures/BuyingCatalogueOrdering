using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Infrastructure
{
    public static  class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T item, string name = "arg") where T : class
            => item ?? throw new ArgumentNullException(name);

    }
}
