using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal static class OrderingApiHealthCheck
    {
        internal static async Task AwaitApiRunningAsync(Settings settings)
        {
            var baseUrl = settings.OrderingApiBaseUrl;
            TimeSpan testTimeOut = TimeSpan.FromSeconds(settings.OrderingApiHealthCheckTimeout);

            await AwaitApiRunningAsync($"{baseUrl}/health/live", testTimeOut);
            await AwaitApiRunningAsync($"{baseUrl}/health/ready", testTimeOut);
        }

        internal static async Task AwaitApiRunningAsync(string url, TimeSpan testTimeOut)
        {
            var started = await HttpClientAwaiter.WaitForGetAsync(url, testTimeOut);
            if (!started)
            {
                throw new TimeoutException($"Start Ordering API failed, could not get a successful health status from '{url}' after trying for '{testTimeOut}'");
            }
        }
    }
}
