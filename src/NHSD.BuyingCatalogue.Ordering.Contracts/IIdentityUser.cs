using System;
using System.Collections.Generic;
using System.Text;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface IIdentityUser
    {
        Guid Id { get; }

        string Name { get; }

        void Deconstruct(out Guid id, out string name);
    }
}
