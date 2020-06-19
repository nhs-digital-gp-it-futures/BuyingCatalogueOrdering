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
