using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderItemRecipientsSteps
    {
        private readonly OrderContext orderContext;
        private readonly Settings settings;

        public OrderItemRecipientsSteps(OrderContext orderContext, Settings settings)
        {
            this.orderContext = orderContext;
            this.settings = settings;
        }

        [Given(@"order item recipients exist")]
        public async Task GivenOrderItemRecipientsExist(Table table)
        {
            foreach (var item in table.CreateSet<OrderItemRecipientEntity>())
            {
                var key = (item.OrderId, item.CatalogueItemId);
                var refList = orderContext.OrderItemRecipientsReferenceList;
                if (!refList.ContainsKey((item.OrderId, item.CatalogueItemId)))
                    refList.Add(key, new List<OrderItemRecipientEntity>());

                await item.InsertAsync(settings.ConnectionString);

                refList[key].Add(item);
            }
        }
    }
}
