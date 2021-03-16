using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class PricingUnitSteps
    {
        private readonly OrderContext orderContext;
        private readonly Settings settings;

        public PricingUnitSteps(OrderContext orderContext, Settings settings)
        {
            this.orderContext = orderContext;
            this.settings = settings;
        }

        [Given(@"pricing units exist")]
        public async Task GivenPricingUnitsExist(Table table)
        {
            foreach (var item in table.CreateSet<PricingUnitEntity>())
            {
                await item.InsertAsync(settings.ConnectionString);
                orderContext.PricingUnitReferenceList.Add(item.Name, item);
            }
        }
    }
}
