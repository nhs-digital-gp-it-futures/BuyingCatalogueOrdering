using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class TimeUnit : IEquatable<TimeUnit>
    {
        public static readonly TimeUnit PerMonth = new TimeUnit(1, "month", "per month");
        public static readonly TimeUnit PerYear = new TimeUnit(2, "year", "per year");

        private TimeUnit(
            int id, 
            string name, 
            string description)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        internal static IEnumerable<TimeUnit> List() => 
            new[] { PerMonth, PerYear };

        public static TimeUnit FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return List().SingleOrDefault(
                timeUnit => name.Equals(timeUnit.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static TimeUnit FromId(int id) => 
            List().SingleOrDefault(timeUnit => id == timeUnit.Id);

        public bool Equals(TimeUnit other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as TimeUnit);

        public override int GetHashCode() => Id;
    }
}
