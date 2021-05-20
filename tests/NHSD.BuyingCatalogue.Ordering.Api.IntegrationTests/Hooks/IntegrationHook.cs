using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Hooks
{
    [Binding]
    internal sealed class IntegrationHook
    {
        // ReSharper disable once StringLiteralTypo
        private static readonly Lazy<IConfigurationRoot> ConfigRoot = new(() => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build());

        private readonly IObjectContainer objectContainer;

        public IntegrationHook(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
        }

        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            var settings = new Settings(ConfigRoot.Value);
            await OrderingApiHealthCheck.AwaitApiRunningAsync(settings);

            // From the documentation on reseeding the current identity value: if no rows have been inserted into the
            // table since the table was created, or if all rows have been removed by using the TRUNCATE TABLE
            // statement, the first row inserted after you run DBCC CHECKIDENT uses new_reseed_value as the identity.
            // If rows are present in the table, or if all rows have been removed by using the DELETE statement, the
            // next row inserted uses new_reseed_value + the current increment value.
            // https://docs.microsoft.com/en-us/sql/t-sql/database-console-commands/dbcc-checkident-transact-sql
            //
            // Therefore, to make sure the identity value is consistent across all tests we insert an order before the
            // test run. This is simpler than dropping all FK constraints to allow a TRUNCATE statement followed by
            // recreating all FK constraints.
            await CreateDummyOrderForConsistentIdentityReseed(settings);
        }

        [BeforeScenario]
        public async Task BeforeScenarioAsync()
        {
            RegisterTestConfiguration();
            RegisterCustomValueRetrievers();
            await ResetDatabaseAsync();
        }

        [AfterScenario]
        public async Task CleanUpAsync()
        {
            await DeleteAllSentEmailsAsync();
        }

        public void RegisterTestConfiguration()
        {
            var config = ConfigRoot.Value;

            objectContainer.RegisterInstanceAs<IConfiguration>(config);
            objectContainer.RegisterInstanceAs(new EmailServerDriverSettings(config.GetValue<Uri>("SmtpServerApiBaseUrl")));
        }

        private static async Task CreateDummyOrderForConsistentIdentityReseed(Settings settings)
        {
            // Included in case of an error during the first scenario that could leave trailing data
            // and cause a foreign key violation when the dummy ordering party is inserted
            await ResetDatabaseAsync();

            var party = new OrderingPartyEntity
            {
                Id = Guid.NewGuid(),
                Name = "Dummy IDENTITY reseed party",
                OdsCode = "ABC",
            };

            var now = DateTime.UtcNow;
            var order = new OrderEntity
            {
                Id = 10001,
                Description = "Dummy order for consistent IDENTITY reseed",
                OrderingPartyId = party.Id,
                OrderStatus = OrderStatus.Incomplete,
                Created = now,
                LastUpdated = now,
                LastUpdatedBy = Guid.Empty,
            };

            await party.InsertAsync(settings.ConnectionString);
            await order.InsertAsync(settings.OrderingDbAdminConnectionString);
        }

        private static void RegisterCustomValueRetrievers()
        {
            var valueRetrievers = Service.Instance.ValueRetrievers;

            valueRetrievers.Register(new DateTimeValueRetriever());
            valueRetrievers.Register(new GenerateStringLengthValueRetriever());
            valueRetrievers.Register(new NullStringValueRetriever());
        }

        private static async Task ResetDatabaseAsync() => await IntegrationDatabase.ResetAsync(ConfigRoot.Value);

        private async Task DeleteAllSentEmailsAsync()
        {
            var emailServerDriver = objectContainer.Resolve<EmailServerDriver>();
            await emailServerDriver.ClearAllEmailsAsync();
        }
    }
}
