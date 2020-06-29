namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CatalogueSolutionModel
    {
        public int OrderItemId { get; set; }
        public string SolutionName { get; set; }
        public GetServiceRecipientModel ServiceRecipient { get; set; }
    }
}
