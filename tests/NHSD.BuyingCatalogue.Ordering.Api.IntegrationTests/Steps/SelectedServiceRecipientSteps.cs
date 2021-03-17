using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class SelectedServiceRecipientSteps
    {
        private readonly Settings settings;

        public SelectedServiceRecipientSteps(Settings settings) => this.settings = settings;

        [Given(@"selected service recipients exist")]
        public async Task GivenSelectedServiceRecipientsExist(Table table)
        {
            foreach (var item in table.CreateSet<SelectedServiceRecipientEntity>())
                await item.InsertAsync(settings.ConnectionString);
        }
    }
}
