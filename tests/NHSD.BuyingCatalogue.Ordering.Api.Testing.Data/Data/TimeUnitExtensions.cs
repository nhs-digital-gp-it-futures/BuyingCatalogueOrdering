using System.Globalization;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data
{
    public static class TimeUnitExtensions
    {
        public static string ToDescription(this TimeUnit timeUnit)
        {
            return $"per {timeUnit}".ToLower(CultureInfo.CurrentCulture);
        }
    }
}
