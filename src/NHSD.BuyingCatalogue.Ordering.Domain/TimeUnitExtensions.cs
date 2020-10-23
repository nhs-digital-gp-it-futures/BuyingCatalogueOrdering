using EnumsNET;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public static class TimeUnitExtensions
    {
        public static string Description(this TimeUnit timeUnit) => timeUnit.AsString(EnumFormat.Description);

        public static string Name(this TimeUnit timeUnit) => timeUnit.AsString(EnumFormat.DisplayName);

        internal static int AmountInYear(this TimeUnit timeUnit) =>
            timeUnit.GetAttributes().Get<AmountInYearAttribute>().AmountInYear;
    }
}
