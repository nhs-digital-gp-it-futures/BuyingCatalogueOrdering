using System;
using EnumsNET;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public static class OrderingEnums
    {
        public static TimeUnit? ParseTimeUnit(string value)
        {
            const int undefinedTimeUnit = 0;

            return Parse<TimeUnit>(value, undefinedTimeUnit, EnumFormat.DisplayName, EnumFormat.Name);
        }

        public static T? Parse<T>(string value)
            where T : struct, Enum
        {
            return Parse<T>(value, null, EnumFormat.Name);
        }

        private static T? Parse<T>(string value, T? valueIfUndefined, params EnumFormat[] formats)
            where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return Enums.TryParse<T>(value, true, out var result, formats) ? result : valueIfUndefined;
        }
    }
}
