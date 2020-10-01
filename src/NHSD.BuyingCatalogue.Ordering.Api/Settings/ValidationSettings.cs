namespace NHSD.BuyingCatalogue.Ordering.Api.Settings
{
    public sealed class ValidationSettings
    {
        public int MaxDeliveryDateWeekOffset { get; set; }

        internal int MaxDeliveryDateOffsetInDays => MaxDeliveryDateWeekOffset * 7;
    }
}
