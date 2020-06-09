using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ServiceRecipient
    {
        [Key]
        public string OdsCode { get; set; }

        [Key]
        public string OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        
        public string Name { get; set; }
    }
}
