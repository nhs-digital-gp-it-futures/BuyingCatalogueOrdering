using System;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IIdentityUser
    {
        Guid Id { get; }

        string Name { get; }

        void Deconstruct(out Guid id, out string name);
    }
}
