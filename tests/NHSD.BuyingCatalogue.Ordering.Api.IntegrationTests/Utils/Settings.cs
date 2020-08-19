using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            Authority = config.GetValue<string>("Authority");
            ConnectionString = config.GetConnectionString("OrderingDb");
            OrderingApiBaseUrl = config.GetValue<string>("OrderingApiBaseUrl");
            OrderingApiHealthCheckTimeout = config.GetValue<int>("OrderingApiHealthCheckTimeout");
            OrderingDbAdminConnectionString = config.GetConnectionString("OrderingDbAdminConnectionString");
            SmtpServerApiBaseUrl = config.GetValue<string>("SmtpServerApiBaseUrl");
        }

        public string Authority { get; }

        public string ConnectionString { get; }

        public string OrderingApiBaseUrl { get; }

        public int OrderingApiHealthCheckTimeout { get; }

        public string OrderingDbAdminConnectionString { get; }

        public string SmtpServerApiBaseUrl { get; set; }
    }
}
