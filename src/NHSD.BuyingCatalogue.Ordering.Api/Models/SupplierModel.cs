namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class SupplierModel
    {
        public string SupplierId { get; set; }

        public string Name { get; set; }

        public AddressModel Address { get; set; }

        public PrimaryContactModel PrimaryContact { get; set; }
    }
}
