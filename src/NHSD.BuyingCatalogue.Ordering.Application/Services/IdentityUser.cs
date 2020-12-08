using System;

namespace NHSD.BuyingCatalogue.Ordering.Application.Services
{
    public sealed class IdentityUser
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
