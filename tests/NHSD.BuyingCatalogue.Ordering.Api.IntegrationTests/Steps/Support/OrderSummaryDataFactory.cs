using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    class OrderSummaryDataFactory
    {
        private readonly Settings _setting;

        OrderSummaryDataFactory(Settings setting)
        {
            _setting = setting;
        }

        public async Task CreateData(string key, string orderId)
        {
            await DataFactory[key](orderId);
        }

        protected IDictionary<string, Func<string,Task>> DataFactory =>
            new Dictionary<string, Func<string,Task>>()
            {
                {"complete", async (orderId) => await  OrderSummaryDataBuilder.CreateOrderSummaryData(orderId).Build().InsertAsync(_setting.ConnectionString)},
                {"complete-with-1recipient-1associatedservice-fundingcomplete", async (orderId) => await  OrderSummaryDataBuilder.CreateOrderSummaryData(orderId)
                    .RemoveAdditionalServices()
                    .RemoveSolutionsServices()
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-0recipient-1associatedservice-fundingcomplete", async (string orderId) => await OrderSummaryDataBuilder.CreateOrderSummaryData(orderId)
                    .RemoveServiceRecipients()
                    .RemoveAdditionalServices()
                    .RemoveSolutionsServices()
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-1associatedservice-fundingcomplete", async (string orderId) => await OrderSummaryDataBuilder.CreateOrderSummaryData(orderId)
                    .RemoveAdditionalServices()
                    .RemoveServiceRecipients()
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-0associatedservice-fundingcomplete", async (string orderId) => await OrderSummaryDataBuilder.CreateOrderSummaryData(orderId)
                    .RemoveAdditionalServices()
                    .RemoveAssociatedServices()
                    .RemoveServiceRecipients()
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-1associatedservice-fundingincomplete", async (string orderId) => await OrderSummaryDataBuilder.CreateOrderSummaryData(orderId)
                    .RemoveOrders()
                    .RemoveAdditionalServices()
                    .RemoveServiceRecipients()
                    .AddOrderEntity(
                        OrderEntityBuilder.Create()
                            .WithOrderStatusId(2)
                            .WithOrderId(orderId)
                            .WithDescription("A Description")
                            .WithOrganisationId(new Guid("4af62b99-638c-4247-875e-965239cd0c48"))
                            .WithServiceRecipientsViewed(false)
                            .WithAdditionalServicesViewed(false)
                            .WithCatalogueSolutionsViewed(true)
                            .WithAssociatedServicesViewed(true)
                            .WithFundingSourceOnlyGMS(null)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                }
            };
    }
}
