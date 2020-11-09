using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    public sealed class ValidationResult
    {
        public ValidationResult(IReadOnlyList<ErrorDetails> errors)
        {
            Errors = errors;
        }

        public IReadOnlyList<ErrorDetails> Errors { get; }

        public bool Success => Errors.Count == 0;
    }
}
