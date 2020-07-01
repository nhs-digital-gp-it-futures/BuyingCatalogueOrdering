using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class SwaggerExtensions
    {
        private const string Version = "v1";

        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(Version, new OpenApiInfo
                {
                    Version = Version,
                    Title = "Ordering API",
                    Description = "NHS Digital GP IT Buying Catalogue HTTP Ordering API"
                });
            });

            return services;
        }

        internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            return UseSwaggerDocumentation(app, PathString.Empty);
        }

        internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, PathString pathBase)
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            string endpointPrefix = !string.IsNullOrWhiteSpace(pathBase) ? pathBase.ToString() : string.Empty;

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{ (endpointPrefix) }/swagger/{Version}/swagger.json", $"Buying Catalogue Ordering API {Version}");
            });

            return app;
        }
    }
}
