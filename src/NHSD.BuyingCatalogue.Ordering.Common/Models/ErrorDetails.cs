using System;
using System.Collections.Generic;
using System.Text;

namespace NHSD.BuyingCatalogue.Ordering.Common.Models
{
    public sealed class ErrorDetails : IEquatable<ErrorDetails>
    {
        public string Id { get; }

        public string? Field { get; }

        public ErrorDetails(string id, string? field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        public bool Equals(ErrorDetails? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                   && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }


        public override bool Equals(object? obj)
        {
            return Equals(obj as ErrorDetails);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Field);

    }
}
