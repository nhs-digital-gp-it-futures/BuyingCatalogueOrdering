using System;
using NHSD.BuyingCatalogue.Ordering.Contracts;

namespace NHSD.BuyingCatalogue.Ordering.Services
{
    public sealed class IdentityUser : IIdentityUser
    {
        public IdentityUser(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }

        public void Deconstruct(out Guid id, out string name)
        {
            id = Id;
            name = Name;
        }
    }
}
