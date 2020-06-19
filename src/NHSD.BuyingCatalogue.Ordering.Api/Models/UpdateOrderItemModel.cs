using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class UpdateOrderItemModel
    {
        public string DeliverDate { get; set; }
        public int Quantity { get; set; }
        public string EstimationPeriod { get; set; }
        public decimal Price { get; set; }
    }
}
