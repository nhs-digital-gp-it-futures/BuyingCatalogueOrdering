using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ErrorDetails
    {
        public string Id { get; }

        public string Field { get; }

        public ErrorDetails(string id) : this(id, null)
        {
        }

        public ErrorDetails(string id, string field)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        private bool Equals(ErrorDetails other)
        {
            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                   && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ErrorDetails other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Field);
        }
    }
}
