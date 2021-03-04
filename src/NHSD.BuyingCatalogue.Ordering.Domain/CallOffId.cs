using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using NHSD.BuyingCatalogue.Ordering.Domain.TypeConverters;
using static System.FormattableString;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    [TypeConverter(typeof(CallOffIdTypeConverter))]
    public readonly struct CallOffId : IEquatable<CallOffId>
    {
        public const int MaxId = 999999;
        public const int MaxRevision = 99;

        private const string Pattern = @"^C(?<id>\d{1,6})-(?<revision>\d{1,2})$";

        private static readonly Lazy<Regex> Regex = new(() => new Regex(Pattern, RegexOptions.Compiled));

        public CallOffId(int id, byte revision)
        {
            if (id < 0 || id > MaxId)
                throw new ArgumentOutOfRangeException(nameof(id));

            if (revision > MaxRevision)
                throw new ArgumentOutOfRangeException(nameof(revision));

            Id = id;
            Revision = revision;
        }

        public int Id { get; }

        public byte Revision { get; }

        public static bool operator ==(CallOffId left, CallOffId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CallOffId left, CallOffId right)
        {
            return !(left == right);
        }

        public static (bool Success, CallOffId Id) Parse(string callOffId)
        {
            var match = Regex.Value.Match(callOffId);
            if (!match.Success)
                return (false, default);

            var id = int.Parse(match.Groups["id"].Value, CultureInfo.InvariantCulture);
            var revision = byte.Parse(match.Groups["revision"].Value, CultureInfo.InvariantCulture);

            return (true, new CallOffId(id, revision));
        }

        public bool Equals(CallOffId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is CallOffId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return Invariant($"C{Id:D6}-{Revision:D2}");
        }
    }
}
