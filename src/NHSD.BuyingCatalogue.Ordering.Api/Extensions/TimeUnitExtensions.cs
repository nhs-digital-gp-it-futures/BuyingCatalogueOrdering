using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class TimeUnitExtensions
    {
        internal static TimeUnitModel ToModel(this TimeUnit timeUnit)
        {
            if (timeUnit is null)
                throw new ArgumentNullException(nameof(timeUnit));

            return new TimeUnitModel
            {
                Name = timeUnit.Name,
                Description = timeUnit.Description
            };
        }
    }
}
