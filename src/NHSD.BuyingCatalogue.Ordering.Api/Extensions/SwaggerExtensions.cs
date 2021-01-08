using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    internal static class SwaggerExtensions
    {
        private const string Version = "v1";

        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            var authorityUrl = configuration.GetValue<string>("Authority");
            var authorizationUrl = new UriBuilder($"{authorityUrl}/connect/authorize").Uri;
            var tokenUrl = new UriBuilder($"{authorityUrl}/connect/token").Uri;

            var openApiSecurityScheme = new OpenApiSecurityScheme
            {
                Description = "Buying Catalogue authentication using identity.",
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = authorizationUrl,
                        TokenUrl = tokenUrl,
                        Scopes = new Dictionary<string, string>
                        {
                            { "Ordering", "Organisation" },
                        },
                    },
                },
            };

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(Version, new OpenApiInfo
                {
                    Version = Version,
                    Title = "Ordering API",
                    Description = "NHS Digital GP IT Buying Catalogue HTTP Ordering API",
                });

                options.AddSecurityDefinition("oauth2", openApiSecurityScheme);

                options.OperationFilter<AuthorizeCheckOperationFilter>();
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

            var endpointPrefix = !string.IsNullOrWhiteSpace(pathBase) ? pathBase.ToString() : string.Empty;

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{endpointPrefix}/swagger/{Version}/swagger.json", $"Buying Catalogue Ordering API {Version}");
            });

            return app;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (!HasAuthorizeAttribute(context))
                {
                    return;
                }

                operation.Responses.TryAdd(
                    StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture),
                    new OpenApiResponse { Description = "Unauthorized" });

                operation.Responses.TryAdd(
                    StatusCodes.Status403Forbidden.ToString(CultureInfo.InvariantCulture),
                    new OpenApiResponse { Description = "Forbidden" });

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "oauth2",
                        Type = ReferenceType.SecurityScheme,
                    },
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new() { [oAuthScheme] = new[] { "orderingapi" } },
                };
            }

            private static bool HasAuthorizeAttribute(OperationFilterContext context)
            {
                // Check for authorize attribute
                var contextMethodInfo = context.MethodInfo;
                var declaringType = contextMethodInfo.DeclaringType;
                if (declaringType is null)
                {
                    return false;
                }

                var hasAuthorize = declaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                                   contextMethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

                return hasAuthorize;
            }
        }
    }
}
