namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    internal sealed class OrganisationModel
    {
        public string Name { get; set; }
        public string OdsCode { get; set; }
        public AddressModel Address { get; set; }
    }
}
