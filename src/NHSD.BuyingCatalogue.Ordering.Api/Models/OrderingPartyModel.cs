namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderingPartyModel
    {
        public string Name { get; set; }
        public string OdsCode { get; set; }
        public AddressModel Address { get; set; }
        public PrimaryContactModel PrimaryContact { get; set; }
    }
}
