using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CatalogueItemSteps
    {
        private readonly OrderContext orderContext;
        private readonly Settings settings;

        public CatalogueItemSteps(OrderContext orderContext, Settings settings)
        {
            this.orderContext = orderContext;
            this.settings = settings;
        }

        [Given(@"catalogue items exist")]
        public async Task GivenCatalogueItemsExist(Table table)
        {
            foreach (var item in table.CreateSet<CatalogueItemEntity>())
            {
                await item.InsertAsync(settings.ConnectionString);
                orderContext.CatalogueItemReferenceList.Add(item.Id, item);
            }
        }
    }
}
