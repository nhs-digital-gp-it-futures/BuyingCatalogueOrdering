using System.Globalization;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data
{
    public enum TimeUnit
    {
        Month = 1,
        Year = 2,
        Invalid = 3
    }

    public static class TimeUnitExtensions
    {
        public static string ToDescription(this TimeUnit timeUnit)
        {
            return $"per {timeUnit}".ToLower(CultureInfo.CurrentCulture);
        }
    }
}
