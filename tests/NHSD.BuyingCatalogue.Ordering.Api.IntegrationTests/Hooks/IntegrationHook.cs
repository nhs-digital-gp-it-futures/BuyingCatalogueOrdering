using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Hooks
{
    [Binding]
    public sealed class IntegrationHook
    {
        private static bool firstScenario = true;
        private readonly IObjectContainer objectContainer;
        private readonly OrderingApiHealthCheck orderingApiHealthCheck;

        public IntegrationHook(IObjectContainer objectContainer, OrderingApiHealthCheck orderingApiHealthCheck)
        {
            this.objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
            this.orderingApiHealthCheck = orderingApiHealthCheck;
        }

        [BeforeScenario]
        public async Task BeforeScenarioAsync()
        {
            RegisterTestConfiguration();
            RegisterCustomValueRetrievers();

            if (firstScenario)
            {
                firstScenario = false;
                await orderingApiHealthCheck.AwaitApiRunningAsync(objectContainer.Resolve<Settings>());
            }

            await ResetDatabaseAsync();
        }

        [AfterScenario]
        public async Task CleanUpAsync()
        {
            await DeleteAllSentEmailsAsync();
        }

        public void RegisterTestConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
            objectContainer.RegisterInstanceAs(
                new EmailServerDriverSettings(configurationBuilder.GetValue<Uri>("SmtpServerApiBaseUrl")));
        }

        private static void RegisterCustomValueRetrievers()
        {
            var valueRetrievers = Service.Instance.ValueRetrievers;

            valueRetrievers.Register(new DateTimeValueRetriever());
            valueRetrievers.Register(new GenerateStringLengthValueRetriever());
            valueRetrievers.Register(new NullStringValueRetriever());
        }

        private async Task ResetDatabaseAsync() =>
            await IntegrationDatabase.ResetAsync(objectContainer.Resolve<IConfiguration>());

        private async Task DeleteAllSentEmailsAsync()
        {
            var emailServerDriver = objectContainer.Resolve<EmailServerDriver>();
            await emailServerDriver.ClearAllEmailsAsync();
        }
    }
}
