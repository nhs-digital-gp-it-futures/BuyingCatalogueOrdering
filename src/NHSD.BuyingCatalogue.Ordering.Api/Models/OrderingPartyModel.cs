namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    internal sealed class OrderingPartyModel
    {
        public OrganisationModel Organisation { get; set; }
        public PrimaryContactModel PrimaryContact { get; set; }
    }
}
