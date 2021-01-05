using NHSD.BuyingCatalogue.EmailClient;

namespace NHSD.BuyingCatalogue.Ordering.Api.Settings
{
    public sealed class PurchasingEmailSettings
    {
        /// <summary>
        /// Gets or sets the recipient of the e-mail message to send to the purchasing system.
        /// </summary>
        public EmailAddressTemplate Recipient { get; set; }

        /// <summary>
        /// Gets or sets the sender, subject and content of the e-mail message to send to the purchasing system.
        /// </summary>
        public EmailMessageTemplate Message { get; set; }
    }
}
