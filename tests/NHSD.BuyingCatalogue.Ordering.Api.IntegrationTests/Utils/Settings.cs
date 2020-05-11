using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("OrderingDbAdmin");
            ConnectionString = config.GetConnectionString("OrderingDb");
            OrderingApiBaseUrl = config.GetValue<string>("OrderingApiBaseUrl");
        }

        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string OrderingApiBaseUrl { get; }
    }
}
