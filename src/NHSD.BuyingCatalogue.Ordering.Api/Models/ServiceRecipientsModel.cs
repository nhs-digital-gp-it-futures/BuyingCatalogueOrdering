using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class ServiceRecipientsModel
    {
        public IEnumerable<ServiceRecipientModel> ServiceRecipients { get; set; }
    }
}
