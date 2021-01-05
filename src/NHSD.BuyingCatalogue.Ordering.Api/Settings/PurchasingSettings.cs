namespace NHSD.BuyingCatalogue.Ordering.Api.Settings
{
    public sealed class PurchasingSettings
    {
        /// <summary>
        /// Gets or sets the sender, subject and content of the e-mail message to send to the purchasing system.
        /// </summary>
        public PurchasingEmailSettings EmailMessage { get; set; }
    }
}
