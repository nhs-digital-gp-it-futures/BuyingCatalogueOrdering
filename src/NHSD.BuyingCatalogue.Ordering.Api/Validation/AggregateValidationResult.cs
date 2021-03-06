﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Validation
{
    public sealed class AggregateValidationResult
    {
        private readonly Dictionary<int, ValidationResult> failedValidations = new();

        public IReadOnlyDictionary<int, ValidationResult> FailedValidations => failedValidations;

        public bool Success { get; private set; } = true;

        public void AddValidationResult(ValidationResult result, int index)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (result.Success)
                return;

            if (failedValidations.TryGetValue(index, out var existingResult))
                failedValidations[index] = new ValidationResult(existingResult, result);
            else
                failedValidations.Add(index, result);

            Success = false;
        }

        public IEnumerable<(string Key, string ErrorMessage)> ToModelErrors()
        {
            return failedValidations.SelectMany(k => ToModelErrors(k.Key, k.Value.Errors));
        }

        private static IEnumerable<(string Key, string ErrorMessage)> ToModelErrors(int key, IEnumerable<ErrorDetails> errors)
        {
            return errors.Select(e => (CreateModelKey(e.ParentName, key, e.Field), e.Id));
        }

        private static string CreateModelKey(string parentName, int key, string field)
        {
            return ModelNames.CreatePropertyModelName(
                ModelNames.CreateIndexModelName(parentName, key),
                field);
        }
    }
}
