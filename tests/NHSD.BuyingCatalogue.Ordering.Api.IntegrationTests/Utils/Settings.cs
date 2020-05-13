using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            OrderingDbAdminConnectionString = config.GetConnectionString("OrderingDbAdminConnectionString");
            ConnectionString = config.GetConnectionString("OrderingDb");
            OrderingApiBaseUrl = config.GetValue<string>("OrderingApiBaseUrl");
        }

        public string OrderingDbAdminConnectionString { get; }

        public string ConnectionString { get; }

        public string OrderingApiBaseUrl { get; }
    }
}
