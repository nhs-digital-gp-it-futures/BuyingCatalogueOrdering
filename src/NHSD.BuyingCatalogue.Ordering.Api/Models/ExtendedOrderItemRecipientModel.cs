namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class ExtendedOrderItemRecipientModel : OrderItemRecipientModel
    {
        public string ItemId { get; init; }

        public string ServiceInstanceId { get; init; }
    }
}
