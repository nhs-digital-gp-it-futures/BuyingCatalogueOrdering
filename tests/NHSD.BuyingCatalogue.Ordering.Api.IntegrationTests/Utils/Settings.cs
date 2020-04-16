using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    public sealed class Settings
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
