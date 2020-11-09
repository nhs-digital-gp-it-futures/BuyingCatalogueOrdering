using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    public sealed class AggregateValidationResult
    {
        private readonly Dictionary<int, ValidationResult> failedValidations = new Dictionary<int, ValidationResult>();

        public bool Success { get; private set; } = true;

        public void AddValidationResult(ValidationResult result, int index)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (result.Success)
                return;

            failedValidations.Add(index, result);
            Success = false;
        }

        public IEnumerable<(string Key, string ErrorMessage)> ToModelErrors()
        {
            return failedValidations.SelectMany(k => ToModelErrors(k.Key, k.Value.Errors));
        }

        private static IEnumerable<(string Key, string ErrorMessage)> ToModelErrors(int key, IEnumerable<ErrorDetails> errors)
        {
            return errors.Select(e => (CreateModelKey(key, e.Field), e.Id));
        }

        private static string CreateModelKey(int key, string field)
        {
            return ModelNames.CreatePropertyModelName(
                ModelNames.CreateIndexModelName(string.Empty, key),
                field);
        }
    }
}
