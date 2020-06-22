using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class TimeUnit
    {
        public static readonly TimeUnit PerMonth = new TimeUnit(1, "month", "per month");
        public static readonly TimeUnit PerYear = new TimeUnit(2, "year", "per year");

        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        private TimeUnit(
            int id, 
            string name, 
            string description)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        internal static IEnumerable<TimeUnit> List() => 
            new[] { PerMonth, PerYear };

        internal static TimeUnit FromName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var timeUnit = List()
                .SingleOrDefault(s => name.Equals(s.Name, StringComparison.CurrentCultureIgnoreCase));

            return timeUnit;
        }
    }
}
