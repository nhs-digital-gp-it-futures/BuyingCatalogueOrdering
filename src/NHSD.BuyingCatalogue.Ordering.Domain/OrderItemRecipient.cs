using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderItemRecipient
    {
        public DateTime? DeliveryDate { get; set; }

        public int Quantity { get; set; }

        public ServiceRecipient Recipient { get; set; }

        public decimal CalculateTotalCostPerYear(decimal price, TimeUnit? timePeriod)
        {
            return price * Quantity * (timePeriod?.AmountInYear() ?? 1);
        }
    }
}
