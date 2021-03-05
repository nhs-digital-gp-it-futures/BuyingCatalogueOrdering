using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class SupplierModel
    {
        public SupplierModel()
        {
        }

        internal SupplierModel(Supplier supplier, Contact contact)
        {
            SupplierId = supplier?.Id;
            Name = supplier?.Name;
            Address = supplier?.Address.ToModel();
            PrimaryContact = contact?.ToModel();
        }

        public string SupplierId { get; init; }

        public string Name { get; init; }

        public AddressModel Address { get; init; }

        public PrimaryContactModel PrimaryContact { get; init; }
    }
}
