namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ServiceInstanceItem
    {
        public int OrderId { get; set; }

        public string CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public string ServiceInstanceId { get; set; }
    }
}
