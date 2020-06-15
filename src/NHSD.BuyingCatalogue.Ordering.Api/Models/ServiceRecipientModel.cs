using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class ServiceRecipientModel : IEquatable<ServiceRecipientModel>
    {
        public string Name { get; set; }
        public string OdsCode { get; set; }

        public bool Equals(ServiceRecipientModel other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is object
                   && string.Equals(OdsCode, other.OdsCode, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ServiceRecipientModel);
        }

        public override int GetHashCode() => HashCode.Combine(OdsCode);
    }
}

