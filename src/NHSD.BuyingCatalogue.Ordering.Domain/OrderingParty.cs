using System;
using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderingParty
    {
        private readonly List<Order> orders = new();

        public Guid Id { get; init; }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public IReadOnlyCollection<Order> Orders => orders.AsReadOnly();
    }
}
