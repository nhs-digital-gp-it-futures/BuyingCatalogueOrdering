namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CatalogueSolutionModel
    {
        public string CatalogueItemId { get; set; }
        public int OrderItemId { get; set; }
        public string SolutionName { get; set; }
        public GetServiceRecipientModel ServiceRecipient { get; set; }
    }
}
