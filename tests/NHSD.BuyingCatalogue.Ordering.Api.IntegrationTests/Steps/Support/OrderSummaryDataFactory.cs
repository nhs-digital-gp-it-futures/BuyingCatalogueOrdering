using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    internal sealed class OrderSummaryDataFactory
    {
        private readonly Settings _setting;

        internal OrderSummaryDataFactory(Settings setting)
        {
            _setting = setting;
        }

        public async Task CreateData(string key, string orderId)
        {
            await DataFactory[key](orderId);
        }

        public IDictionary<string, Func<string,Task>> DataFactory =>
            new Dictionary<string, Func<string,Task>>()
            {
                {"complete", async (orderId) => await  OrderSummaryDataBuilder.Create(orderId).Build().InsertAsync(_setting.ConnectionString)},
                {"complete-with-1recipient-1associatedservice-fundingcomplete", async (orderId) => await  OrderSummaryDataBuilder.Create(orderId)
                    .WithAdditionalServicesEntities(null)
                    .WithCatalogueSolutionEntities(null)
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-0recipient-1associatedservice-fundingcomplete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithServiceRecipientEntities(null)
                    .WithAdditionalServicesEntities(null)
                    .WithCatalogueSolutionEntities(null)
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-1associatedservice-fundingcomplete", async ( orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithAdditionalServicesEntities(null)
                    .WithServiceRecipientEntities(null)
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-0associatedservice-fundingcomplete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithAdditionalServicesEntities(null)
                    .WithAssociatedServicesEntities(null)
                    .WithServiceRecipientEntities(null)
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-1associatedservice-fundingincomplete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithOrderEntity(
                        OrderEntityBuilder.Create()
                            .WithOrderStatusId((int)OrderStatus.Unsubmitted)
                            .WithOrderId(orderId)
                            .WithDescription("A Description")
                            .WithOrganisationId(new Guid("4af62b99-638c-4247-875e-965239cd0c48"))
                            .WithServiceRecipientsViewed(false)
                            .WithAdditionalServicesViewed(false)
                            .WithCatalogueSolutionsViewed(true)
                            .WithAssociatedServicesViewed(true)
                            .WithFundingSourceOnlyGMS(null)
                            .Build())
                    .WithAdditionalServicesEntities(null)
                    .WithServiceRecipientEntities(null)
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                }
            };
    }
}
