using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderDescription : ValueObject
    {
        private OrderDescription()
        {
        }

        private OrderDescription(string value)
            : this()
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<OrderDescription> Create(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return Result.Failure<OrderDescription>(OrderErrors.OrderDescriptionRequired());
            }

            return description.Length > 100
                ? Result.Failure<OrderDescription>(OrderErrors.OrderDescriptionTooLong())
                : Result.Success(new OrderDescription(description));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
