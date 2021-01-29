﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ServiceRecipient : IEquatable<ServiceRecipient>
    {
        public ServiceRecipient()
        {
        }

        public ServiceRecipient(string orderId, string odsCode, string name)
        {
            OdsCode = odsCode;
            OrderId = orderId;
            Name = name;
        }

        public string OdsCode { get; init; }

        public string OrderId { get; init; }

        [ForeignKey("OrderId")]
        public Order Order { get; init; }

        public string Name { get; init; }

        public bool Equals(ServiceRecipient other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return OdsCode.EqualsOrdinalIgnoreCase(other.OdsCode)
                && OrderId.EqualsOrdinalIgnoreCase(other.OrderId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ServiceRecipient);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                OdsCode?.ToUpperInvariant(),
                OrderId?.ToUpperInvariant());
        }
    }
}
