using System;
using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderModel
    {
        public string Description { get; set; }
        public OrderingPartyModel OrderParty { get; set; }
        public SupplierModel Supplier { get; set; }
        public DateTime? CommencementDate { get; set; }
        public IEnumerable<OrderItemModel> OrderItems { get; set; }
        public IEnumerable<ServiceRecipientModel> ServiceRecipients { get; set; }
        public decimal TotalOneOffCost { get; set; }
        public decimal TotalRecurringCostPerMonth { get; set; }
        public decimal TotalRecurringCostPerYear { get; set; }
        public decimal TotalOwnershipCost { get; set; }
        public string Status { get; set; }
    }
}
