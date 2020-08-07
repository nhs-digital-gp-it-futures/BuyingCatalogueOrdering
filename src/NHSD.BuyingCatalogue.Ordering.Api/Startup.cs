using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSD.BuyingCatalogue.Ordering.Api.ActionFilters;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Logging;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NHSD.BuyingCatalogue.Ordering.Persistence.Repositories;
using Serilog;

namespace NHSD.BuyingCatalogue.Ordering.Api
{
    public sealed class Startup
    {
        private readonly IWebHostEnvironment _environment;

        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("OrderingDb");
            var authority = _configuration.GetValue<string>("Authority");
            var requireHttps = _configuration.GetValue<bool>("RequireHttps");
            var allowInvalidCertificate = _configuration.GetValue<bool>("AllowInvalidCertificate");
            var bypassIdentity = _configuration.GetValue<bool>("BypassIdentity");

            var validationSettings = new ValidationSettings
            {
                MaxDeliveryDateWeekOffset = _configuration.GetValue<int>("MaxDeliveryDateWeekOffset")
            };

            Log.Logger.Information("Authority on ORDAPI is: {@authority}", authority);
            Log.Logger.Information("ORDAPI Require Https: {@requiredHttps}", requireHttps);
            Log.Logger.Information($"ORDAPI Allow Invalid Certificates: {@allowInvalidCertificate}", allowInvalidCertificate);
            Log.Logger.Information("ORDAPI BypassIdentity: {@bypassIdentity}", bypassIdentity);
            
            IdentityModelEventSource.ShowPII = _environment.IsDevelopment();

            services.AddHttpContextAccessor();
            services.AddTransient<IServiceRecipientRepository, ServiceRecipientRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddSingleton(validationSettings);

            services
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<ICreateOrderService, CreateOrderService>()
                .AddTransient<ICreateOrderItemService, CreateOrderItemService>()
                .AddTransient<IUpdateOrderItemService, UpdateOrderItemService>()
                .AddTransient<ICreateOrderItemValidator, OrderItemValidator>()
                .AddTransient<IUpdateOrderItemValidator, OrderItemValidator>();

            services.RegisterHealthChecks(connectionString);

            services.AddSwaggerDocumentation();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = requireHttps;
                    options.Audience = "Ordering";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name"
                    };

                    if (bypassIdentity)
                    {
                        options.BypassIdentity();
                    }

                    if (allowInvalidCertificate)
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                    }
                });

            services.AddControllers(options => options.Filters.Add<InputValidationActionFilter>())
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString).EnableSensitiveDataLogging(_environment.IsDevelopment());
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyName.CanAccessOrders,
                    policyBuilder =>
                    {
                        policyBuilder.RequireClaim(ApplicationClaimTypes.Ordering);
                    });

                options.AddPolicy(PolicyName.CanManageOrders,
                    policyBuilder =>
                    {
                        policyBuilder.RequireClaim(ApplicationClaimTypes.Ordering, ApplicationPermissions.Manage);
                    });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = SerilogRequestLoggingOptions.GetLevel;
            });

            var pathBase = _configuration.GetValue<string>("PathBase");
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                Log.Logger.Information($"USING PATH BASE {pathBase}");
                app.UsePathBase(pathBase);
            }

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerDocumentation(pathBase);
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Live)
                });

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Ready)
                });
            });
        }
    }
}
