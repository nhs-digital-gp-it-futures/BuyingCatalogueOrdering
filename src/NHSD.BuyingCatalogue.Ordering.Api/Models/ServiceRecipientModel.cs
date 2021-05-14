using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class ServiceRecipientModel : IEquatable<ServiceRecipientModel>, IServiceRecipient
    {
        public ServiceRecipientModel()
        {
        }

        public string Name { get; init; }

        public string OdsCode { get; init; }

        public bool Equals(ServiceRecipientModel other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || string.Equals(OdsCode, other.OdsCode, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ServiceRecipientModel);
        }

        public override int GetHashCode() => HashCode.Combine(OdsCode);
    }
}
