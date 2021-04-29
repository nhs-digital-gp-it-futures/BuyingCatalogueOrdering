using System.Net.Http;
using FluentValidation;
using FluentValidation.AspNetCore;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.EmailClient.Configuration;
using NHSD.BuyingCatalogue.Ordering.Api.ActionFilters;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Logging;
using NHSD.BuyingCatalogue.Ordering.Api.ModelBinding;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NHSD.BuyingCatalogue.Ordering.Services;
using Serilog;

namespace NHSD.BuyingCatalogue.Ordering.Api
{
    public sealed class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = configuration.GetConnectionString("OrderingDb");
            var authority = configuration.GetValue<string>("Authority");
            var requireHttps = configuration.GetValue<bool>("RequireHttps");
            var allowInvalidCertificate = configuration.GetValue<bool>("AllowInvalidCertificate");
            var bypassIdentity = configuration.GetValue<bool>("BypassIdentity");
            var validationSettings = new ValidationSettings
            {
                MaxDeliveryDateWeekOffset = configuration.GetValue<int>("MaxDeliveryDateWeekOffset"),
            };

            var smtpSettings = configuration.GetSection("SmtpServer").Get<SmtpSettings>();
            smtpSettings.AllowInvalidCertificate ??= allowInvalidCertificate;

            var purchasingSettings = configuration.GetSection("Purchasing").Get<PurchasingSettings>();

            Log.Logger.Information("Authority on ORDAPI is: {@authority}", authority);
            Log.Logger.Information("ORDAPI Require Https: {@requiredHttps}", requireHttps);
            Log.Logger.Information("ORDAPI Allow Invalid Certificates: {@allowInvalidCertificate}", allowInvalidCertificate);
            Log.Logger.Information("ORDAPI BypassIdentity: {@bypassIdentity}", bypassIdentity);
            Log.Logger.Information("SMTP settings: {@smtpSettings}", smtpSettings);
            Log.Logger.Information("Purchasing settings: {@purchasingSettings}", purchasingSettings);

            IdentityModelEventSource.ShowPII = environment.IsDevelopment();
            ValidatorOptions.Global.DisplayNameResolver = FluentValidationOptions.DisplayNameResolver;

            services.AddHttpContextAccessor();

            services
                .AddSingleton<IValidator<CreateOrderModel>, CreateOrderModelValidator>()
                .AddSingleton<IValidator<OrderDescriptionModel>, OrderDescriptionModelValidator>()
                .AddSingleton<IValidator<ServiceRecipientModel>, ServiceRecipientModelValidator>()
                .AddSingleton<IValidator<CreateOrderItemModel>, CreateOrderItemModelValidator>()
                .AddSingleton<IValidator<OrderItemRecipientModel>, OrderItemRecipientModelValidator>();

            services
                .AddScoped<IMailTransport, SmtpClient>()
                .AddScoped<IEmailService, MailKitEmailService>()
                .AddScoped<IIdentityService, IdentityService>()
                .AddScoped<IContactDetailsService, ContactDetailsService>()
                .AddScoped<ICreateOrderItemService, CreateOrderItemService>()
                .AddScoped<ICompleteOrderService, CompleteOrderService>()
                .AddScoped<IServiceRecipientService, ServiceRecipientService>()
                .AddScoped<ICreatePurchasingDocumentService, CreatePurchasingDocumentService>()
                .AddScoped<ICsvStreamWriter<OdooPatientNumbersOrderItem>, CsvStreamStreamWriter<OdooPatientNumbersOrderItem, OdooPatientNumbersOrderItemMap>>()
                .AddScoped<ICsvStreamWriter<OdooOrderItem>, CsvStreamStreamWriter<OdooOrderItem, OdooOrderItemMap>>()
                .AddScoped<IDefaultDeliveryDateValidator, DefaultDeliveryDateValidator>()
                .AddScoped<ICreateOrderItemValidator, OrderItemValidator>()
                .AddScoped<IAsyncAuthorizationFilter, OrderLookupOrganisationAuthorizationFilter>()
                .AddScoped<ICommencementDateService, CommencementDateService>()
                .AddScoped<IFundingSourceService, FundingSourceService>()
                .AddScoped<IDefaultDeliveryDateService, DefaultDeliveryDateService>()
                .AddScoped<IOrderDescriptionService, OrderDescriptionService>()
                .AddScoped<IOrderingPartyService, OrderingPartyService>()
                .AddScoped<ISectionStatusService, SectionStatusService>()
                .AddScoped<ISupplierSectionService, SupplierSectionService>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IOrderItemService, OrderItemService>();

            services
                .AddSingleton(smtpSettings)
                .AddSingleton(validationSettings)
                .AddScoped(_ => configuration.GetSection("Purchasing").Get<PurchasingSettings>());

            services.RegisterHealthChecks(connectionString, smtpSettings);

            services.AddSwaggerDocumentation(configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = requireHttps;
                    options.Audience = "Ordering";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
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
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        };
                    }
                });

            static void ControllerOptions(MvcOptions options)
            {
                options.Filters.Add<OrderLookupOrganisationAuthorizationFilter>();
                options.Filters.Add<InputValidationActionFilter>();
                options.ModelBinderProviders.Insert(0, new OrderModelBinderProvider());
            }

            services.AddControllers(ControllerOptions)
                .AddFluentValidation()
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString).EnableSensitiveDataLogging(environment.IsDevelopment());
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyName.CanAccessOrders,
                    policyBuilder =>
                    {
                        policyBuilder.RequireClaim(ApplicationClaimTypes.Ordering);
                    });

                options.AddPolicy(
                    PolicyName.CanManageOrders,
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

            var pathBase = configuration.GetValue<string>("PathBase");
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                Log.Logger.Information($"USING PATH BASE {pathBase}");
                app.UsePathBase(pathBase);
            }

            if (environment.IsDevelopment())
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
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Live),
                });

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Ready),
                });
            });
        }
    }
}
