namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class CreateOrderItemModel
    {
        public ServiceRecipientModel ServiceRecipient { get; set; }
        public string SolutionId { get; set; }
        public string DeliveryDate { get; set; }
        public int Quantity { get; set; }
        public string EstimationPeriod { get; set; }
        public string ProvisioningType { get; set; }
        public string Type { get; set; }
        public string CurrencyCode { get; set; }
        public ItemUnitModel ItemUnitModel { get; set; }
        public decimal Price { get; set; }
    }
}
