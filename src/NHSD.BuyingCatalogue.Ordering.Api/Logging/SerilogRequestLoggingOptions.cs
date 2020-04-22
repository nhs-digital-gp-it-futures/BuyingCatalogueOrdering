using System;
using Microsoft.AspNetCore.Http;
using Serilog.Events;

namespace NHSD.BuyingCatalogue.Ordering.Api.Logging
{
    public static class SerilogRequestLoggingOptions
    {
        internal const string HealthCheckEndpointDisplayName = "Health checks";

        internal static LogEventLevel GetLevel(HttpContext httpContext, double _, Exception exception)
        {
            if (exception != null)
                return LogEventLevel.Error;

            if (httpContext == null || httpContext.Response.StatusCode > 499)
                return LogEventLevel.Error;

            return IsHealthCheck(httpContext)
                ? LogEventLevel.Verbose
                : LogEventLevel.Information;
        }

        private static bool IsHealthCheck(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();

            return endpoint != null && string.Equals(
                endpoint.DisplayName,
                HealthCheckEndpointDisplayName,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
