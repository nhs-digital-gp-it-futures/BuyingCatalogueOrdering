using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class OrderItemModel
    {
        public ServiceRecipientModel serviceRecipientModel { get; set; }
        public string CatalogueItemId { get; set; }
        public string DeliverDate { get; set; }
        public int Quantity { get; set; }
        public string EstimationPeriod { get; set; }
        public string ProvisioningType { get; set; }
        public string Type { get; set; }
        public string CurrencyCode { get; set; }
        public ItemUnit ItemUnit { get; set; }
        public decimal Price { get; set; }
    }
}
