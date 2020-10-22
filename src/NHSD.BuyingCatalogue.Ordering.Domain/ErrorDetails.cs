using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ErrorDetails : IEquatable<ErrorDetails>
    {
        public ErrorDetails(string id)
            : this(id, null)
        {
        }

        public ErrorDetails(string id, string field)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        public string Id { get; }

        public string Field { get; }

        public bool Equals(ErrorDetails other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ErrorDetails);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Field);
        }
    }
}
