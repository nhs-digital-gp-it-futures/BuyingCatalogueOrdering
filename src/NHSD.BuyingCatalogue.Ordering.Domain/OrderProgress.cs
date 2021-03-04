namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderProgress
    {
        public int OrderId { get; init; }

        public bool ServiceRecipientsViewed { get; set; }

        public bool CatalogueSolutionsViewed { get; set; }

        public bool AdditionalServicesViewed { get; set; }

        public bool AssociatedServicesViewed { get; set; }
    }
}
