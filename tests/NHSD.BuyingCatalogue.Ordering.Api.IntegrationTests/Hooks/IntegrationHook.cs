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
        private readonly IObjectContainer _objectContainer;
        private readonly OrderingApiHealthCheck _orderingApiHealthCheck;
        private static bool _firstScenario = true;

        public IntegrationHook(IObjectContainer objectContainer, OrderingApiHealthCheck orderingApiHealthCheck)
        {
            _objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
            _orderingApiHealthCheck = orderingApiHealthCheck;
        }

        [BeforeScenario]
        public async Task BeforeScenarioAsync()
        {
            RegisterTestConfiguration();
            RegisterCustomValueRetrievers();

            if (_firstScenario)
            {
                _firstScenario = false;
                await _orderingApiHealthCheck.AwaitApiRunningAsync(_objectContainer.Resolve<Settings>());
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

            _objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
            _objectContainer.RegisterInstanceAs(
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
            await IntegrationDatabase.ResetAsync(_objectContainer.Resolve<IConfiguration>());

        private async Task DeleteAllSentEmailsAsync()
        {
            var emailServerDriver = _objectContainer.Resolve<EmailServerDriver>();
            await emailServerDriver.ClearAllEmailsAsync();
        }
    }
}
