namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderingPartyModel
    {
        public OrganisationModel Organisation { get; set; }
        public PrimaryContactModel PrimaryContact { get; set; }
    }
}
