using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal sealed class Settings
    {
        public string ConnectionString { get; }

        public string OrderingApiBaseUrl { get; }

        public Settings(IConfiguration config)
        {   
            ConnectionString = config.GetConnectionString("OrderingDb");
            OrderingApiBaseUrl = config.GetValue<string>("OrderingApiBaseUrl");
        }
    }
}
