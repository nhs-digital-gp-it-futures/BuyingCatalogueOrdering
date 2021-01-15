using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain.Results
{
    public sealed class Result<T> : IEquatable<Result<T>>
    {
        internal Result(bool isSuccess, IEnumerable<ErrorDetails> errors, T value)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorDetails>(errors?.ToList() ?? new List<ErrorDetails>());
            Value = value;
        }

        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorDetails> Errors { get; }

        public T Value { get; }

        public Result ToResult()
        {
            if (IsSuccess)
                return Result.Success();

            return Result.Failure(Errors);
        }

        public bool Equals(Result<T> other)
        {
            return other is object
                   && IsSuccess == other.IsSuccess
                   && AreErrorsEqual(Errors, other.Errors)
                   && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is Result<T> other && Equals(other));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsSuccess, Errors, Value);
        }

        private static bool AreErrorsEqual(IEnumerable<ErrorDetails> first, IEnumerable<ErrorDetails> second)
        {
            if (first is null)
                return second is null;

            if (second is null)
                return false;

            return first.SequenceEqual(second);
        }
    }
}
