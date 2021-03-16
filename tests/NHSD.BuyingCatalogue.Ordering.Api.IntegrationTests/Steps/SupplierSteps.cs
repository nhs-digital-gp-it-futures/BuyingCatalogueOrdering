using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class SupplierSteps
    {
        private readonly OrderContext orderContext;
        private readonly Settings settings;

        public SupplierSteps(OrderContext orderContext, Settings settings)
        {
            this.orderContext = orderContext;
            this.settings = settings;
        }

        [Given(@"suppliers exist")]
        public async Task GivenSuppliersExist(Table table)
        {
            foreach (var item in table.CreateSet<SupplierEntity>())
            {
                await item.InsertAsync(settings.ConnectionString);
                orderContext.SupplierReferenceList.Add(item.Id, item);
            }
        }
    }
}
