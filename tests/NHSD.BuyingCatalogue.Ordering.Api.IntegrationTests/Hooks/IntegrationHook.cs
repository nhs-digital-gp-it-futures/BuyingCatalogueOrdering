using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Hooks
{
    [Binding]
    public sealed class IntegrationHook
    {
        private readonly IObjectContainer _objectContainer;

        public IntegrationHook(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
        }

        [BeforeScenario]
        public async Task BeforeScenarioAsync()
        {
            RegisterTestConfiguration();
            RegisterCustomValueRetrievers();

            await ResetDatabaseAsync();
        }

        public void RegisterTestConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            _objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
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
    }
}
