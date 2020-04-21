using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.BuyingCatalogue.Ordering.Api.Logging;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using Serilog;


namespace NHSD.BuyingCatalogue.Ordering.Api
{
    public sealed class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("OrderingDb");
            var authority = Configuration.GetValue<string>("authority");
            var requireHttps = Configuration.GetValue<bool>("RequireHttps");
            var allowInvalidCertificate = Configuration.GetValue<bool>("AllowInvalidCertificate");

            services.RegisterHealthChecks(connectionString);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = requireHttps;
                    options.Audience = "Ordering";

                    if (allowInvalidCertificate)
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                    }
                });

            services.AddControllers();

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


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public  void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = SerilogRequestLoggingOptions.GetLevel;
            });


            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

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
