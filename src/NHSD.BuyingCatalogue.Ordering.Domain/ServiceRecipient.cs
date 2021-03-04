using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ServiceRecipient : IEquatable<ServiceRecipient>
    {
        public ServiceRecipient(string odsCode, string name)
        {
            OdsCode = odsCode;
            Name = name;
        }

        public string OdsCode { get; init; }

        public string Name { get; set; }

        public bool Equals(ServiceRecipient other)
        {
            if (other is null)
                return false;

            return ReferenceEquals(this, other) || OdsCode.Equals(other.OdsCode, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ServiceRecipient);
        }

        public override int GetHashCode()
        {
            return OdsCode.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }
    }
}
