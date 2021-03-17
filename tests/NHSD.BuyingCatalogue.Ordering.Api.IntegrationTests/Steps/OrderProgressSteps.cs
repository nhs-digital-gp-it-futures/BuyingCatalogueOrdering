using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderProgressSteps
    {
        private readonly Settings settings;

        public OrderProgressSteps(Settings settings) => this.settings = settings;

        [Given(@"order progress exists")]
        public async Task GivenOrderProgressExists(Table table)
        {
            foreach (var item in table.CreateSet<OrderProgressEntity>())
                await item.InsertAsync(settings.ConnectionString);
        }
    }
}
