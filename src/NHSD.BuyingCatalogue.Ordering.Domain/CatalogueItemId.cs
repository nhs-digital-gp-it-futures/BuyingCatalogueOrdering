using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using NHSD.BuyingCatalogue.Ordering.Domain.TypeConverters;
using static System.FormattableString;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    [TypeConverter(typeof(CatalogueItemIdTypeConverter))]
    public readonly struct CatalogueItemId : IEquatable<CatalogueItemId>
    {
        public const int MaxItemIdLength = 7;
        public const int MaxSupplierId = 999999;

        private const string Pattern = @"^(?<supplierId>\d{1,6})-(?<itemId>\S{1,7})$";

        private static readonly Lazy<Regex> Regex = new(() => new Regex(Pattern, RegexOptions.Compiled));

        public CatalogueItemId(int supplierId, string itemId)
        {
            if (supplierId < 1 || supplierId > MaxSupplierId)
                throw new ArgumentOutOfRangeException(nameof(supplierId));

            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException($"{nameof(itemId)} is required.", nameof(itemId));

            if (itemId.Length > MaxItemIdLength)
                throw new ArgumentOutOfRangeException(nameof(itemId));

            SupplierId = supplierId;
            ItemId = itemId;
        }

        public int SupplierId { get; }

        public string ItemId { get; }

        public static bool operator ==(CatalogueItemId left, CatalogueItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CatalogueItemId left, CatalogueItemId right)
        {
            return !(left == right);
        }

        public static (bool Success, CatalogueItemId Id) Parse(string catalogueItemId)
        {
            var match = Regex.Value.Match(catalogueItemId);
            if (!match.Success)
                return (false, default);

            var supplierId = int.Parse(match.Groups["supplierId"].Value, CultureInfo.InvariantCulture);
            var itemId = match.Groups["itemId"].Value;

            return (true, new CatalogueItemId(supplierId, itemId));
        }

        public static CatalogueItemId ParseExact(string catalogueItemId)
        {
            (bool success, CatalogueItemId id) = Parse(catalogueItemId);
            if (!success)
                throw new FormatException();

            return id;
        }

        public bool Equals(CatalogueItemId other)
        {
            return SupplierId == other.SupplierId && ItemId == other.ItemId;
        }

        public override bool Equals(object obj)
        {
            return obj is CatalogueItemId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SupplierId, ItemId);
        }

        public override string ToString()
        {
            return Invariant($"{SupplierId}-{ItemId}");
        }
    }
}
