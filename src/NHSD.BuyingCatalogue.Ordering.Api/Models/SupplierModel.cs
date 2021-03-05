using System;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class SupplierModel
    {
        public SupplierModel(Supplier supplier, Contact contact)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            SupplierId = supplier.Id;
            Name = supplier.Name;
            Address = supplier.Address.ToModel();
            PrimaryContact = contact.ToModel();
        }

        public string SupplierId { get; }

        public string Name { get; }

        public AddressModel Address { get; }

        public PrimaryContactModel PrimaryContact { get; }
    }
}
