using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AmountInYearAttribute : Attribute
    {
        public AmountInYearAttribute(int amountInYear)
        {
            AmountInYear = amountInYear;
        }

        public int AmountInYear { get; }
    }
}
