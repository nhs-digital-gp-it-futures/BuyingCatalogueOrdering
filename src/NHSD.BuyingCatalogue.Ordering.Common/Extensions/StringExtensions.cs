using System;

namespace NHSD.BuyingCatalogue.Ordering.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsOrdinalIgnoreCase(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static string TrimAsync(this string input)
        {
            return TrimSuffix(input, "async", StringComparison.OrdinalIgnoreCase);
        }

        private static string TrimSuffix(string input, string suffix, StringComparison comparison)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            return input.EndsWith(suffix, comparison)
                ? input.Substring(0, input.Length - suffix.Length)
                : input;
        }
    }
}
