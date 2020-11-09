using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public static class ProvisioningTypeExtensions
    {
        public static TimeUnit? InferEstimationPeriod(this ProvisioningType provisioningType, TimeUnit? estimationPeriod)
        {
            return provisioningType switch
            {
                ProvisioningType.Patient => TimeUnit.PerMonth,
                ProvisioningType.Declarative => TimeUnit.PerYear,
                ProvisioningType.OnDemand => estimationPeriod,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
