using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class OrderingCustomizationOptions
    {
        internal OrderingCustomizationOptions(
            ProvisioningType provisioningType = null,
            TimeUnit timeUnit = null)
        {
            ProvisioningType = provisioningType;
            TimeUnit = timeUnit;
        }

        internal ProvisioningType ProvisioningType { get; }

        internal TimeUnit TimeUnit { get; }
    }
}
