using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NHSD.BuyingCatalogue.Ordering.Common.Models;

namespace NHSD.BuyingCatalogue.Ordering.Common.Results
{
    public sealed class Result<T> : IEquatable<Result<T>>
    {
        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorDetails> Errors { get; }

        [MaybeNull]
        public T Value { get; }

        internal Result(bool isSuccess, IEnumerable<ErrorDetails>? errors, T value)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorDetails>(errors != null ? errors.ToList() : new List<ErrorDetails>());
            Value = value;
        }

        public Result ToResult()
        {
            if (IsSuccess)
                return Result.Success();

            return Result.Failure(Errors);
        }

        private static bool AreErrorsEqual(IEnumerable<ErrorDetails> first, IEnumerable<ErrorDetails> second)
        {
            if (first is null)
                return second is null;

            if (second is null)
                return false;

            return first.SequenceEqual(second);
        }

        public bool Equals(Result<T>? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return other is object
                   && IsSuccess == other.IsSuccess
                   && AreErrorsEqual(Errors, other.Errors)
                   && Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Result<T>);
        }

        public override int GetHashCode() => HashCode.Combine(IsSuccess, Errors, Value!);
    }

}
