using System.ComponentModel.DataAnnotations.Schema;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ServiceRecipient
    {
        public string OdsCode { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public string Name { get; set; }
    }
}
