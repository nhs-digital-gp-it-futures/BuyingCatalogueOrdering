using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain.Results
{
    public sealed class Result : IEquatable<Result>
    {
        private static readonly Result SuccessfulResult = new();

        private Result()
            : this(true, Enumerable.Empty<ErrorDetails>())
        {
        }

        private Result(IEnumerable<ErrorDetails> errors)
            : this(false, errors)
        {
        }

        private Result(bool isSuccess, IEnumerable<ErrorDetails> errors)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorDetails>(errors != null ? errors.ToList() : new List<ErrorDetails>());
        }

        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorDetails> Errors { get; }

        public static Result Success()
        {
            return SuccessfulResult;
        }

        public static Result<T> Success<T>(T value)
        {
            return new(true, new List<ErrorDetails>(), value);
        }

        public static Result Failure(IEnumerable<ErrorDetails> errors)
        {
            return new(errors);
        }

        public static Result Failure(params ErrorDetails[] errors)
        {
            return new(errors);
        }

        public static Result<T> Failure<T>(params ErrorDetails[] errors)
        {
            return new(false, errors, default);
        }

        public static Result<T> Failure<T>(IEnumerable<ErrorDetails> errors)
        {
            return new(false, errors, default);
        }

        public bool Equals(Result other)
        {
            return other is object
                && IsSuccess == other.IsSuccess
                && AreErrorsEqual(Errors, other.Errors);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is Result other && Equals(other));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsSuccess, Errors);
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
