using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderStatus : IEquatable<OrderStatus>
    {
        public static readonly OrderStatus Complete = new OrderStatus(1, nameof(Complete));
        public static readonly OrderStatus Incomplete = new OrderStatus(2, nameof(Incomplete));

        private OrderStatus(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public int Id { get; }

        public string Name { get; }

        internal static IEnumerable<OrderStatus> List() => 
            new[] { Complete, Incomplete };

        public static OrderStatus FromId(int id) => 
            List().SingleOrDefault(item => id == item.Id);

        public static OrderStatus FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return List()
                .SingleOrDefault(s => 
                    name.Equals(s.Name, StringComparison.OrdinalIgnoreCase));
        }

        public bool Equals(OrderStatus other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as OrderStatus);

        public override int GetHashCode() => Id;
    }
}
