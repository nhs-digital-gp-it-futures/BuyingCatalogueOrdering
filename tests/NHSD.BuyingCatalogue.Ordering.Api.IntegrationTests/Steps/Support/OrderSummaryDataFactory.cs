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
                {"complete", async (orderId) => await  OrderSummaryDataBuilder.Create(orderId)                
                    .WithServiceRecipientEntity(
                        ServiceRecipientBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Od1")
                            .WithName("Service Recipient")
                            .Build())
                    .WithCatalogueSolutionEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods1")
                            .WithCatalogueItemName("Order Item 1 ")
                            .WithCatalogueItemType(CatalogueItemType.Solution)
                            .Build())
                    .WithAssociatedServicesEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods2")
                            .WithCatalogueItemName("Order Item 2 ")
                            .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                            .Build())
                    .WithAdditionalServicesEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods3")
                            .WithCatalogueItemName("Order Item 3 ")
                            .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1recipient-1associatedservice-funding-complete", async (orderId) => await  OrderSummaryDataBuilder.Create(orderId)
                    .WithServiceRecipientEntity(
                        ServiceRecipientBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Od1")
                            .WithName("Service Recipient")
                            .Build())
                    .WithAssociatedServicesEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods2")
                            .WithCatalogueItemName("Order Item 2 ")
                            .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-0recipient-1associatedservice-funding-complete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithAssociatedServicesEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods2")
                            .WithCatalogueItemName("Order Item 2 ")
                            .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-1associatedservice-funding-complete", async ( orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithCatalogueSolutionEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods1")
                            .WithCatalogueItemName("Order Item 1 ")
                            .WithCatalogueItemType(CatalogueItemType.Solution)
                            .Build())
                    .WithAssociatedServicesEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods2")
                            .WithCatalogueItemName("Order Item 2 ")
                            .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"complete-with-1solution-0associatedservice-funding-complete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithCatalogueSolutionEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods1")
                            .WithCatalogueItemName("Order Item 1 ")
                            .WithCatalogueItemType(CatalogueItemType.Solution)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"funding-incomplete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
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
                    .WithCatalogueSolutionEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods1")
                            .WithCatalogueItemName("Order Item 1 ")
                            .WithCatalogueItemType(CatalogueItemType.Solution)
                            .Build())
                    .WithAssociatedServicesEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods2")
                            .WithCatalogueItemName("Order Item 2 ")
                            .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"incomplete-with-0recipient-0solution-0associatedservice-funding-complete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithServiceRecipientViewed(false)
                    .WithCatalogueSolutionsViewed(false)
                    .WithAssociatedServicesViewed(false)
                    .WithAdditionalServiceViewed(false)
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"incomplete-with-1recipient-0solution-0associatedservice-funding-complete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithServiceRecipientViewed(true)
                    .WithCatalogueSolutionsViewed(false)
                    .WithAssociatedServicesViewed(false)
                    .WithAdditionalServiceViewed(false)
                    .WithServiceRecipientEntity(
                        ServiceRecipientBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Od1")
                            .WithName("Service Recipient")
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"incomplete-with-1recipient-1solution-0associatedservice-funding-complete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithServiceRecipientViewed(true)
                    .WithCatalogueSolutionsViewed(true)
                    .WithAssociatedServicesViewed(false)
                    .WithAdditionalServiceViewed(false)
                    .WithServiceRecipientEntity(
                        ServiceRecipientBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Od1")
                            .WithName("Service Recipient")
                            .Build())
                    .WithCatalogueSolutionEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods1")
                            .WithCatalogueItemName("Order Item 1 ")
                            .WithCatalogueItemType(CatalogueItemType.Solution)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"incomplete-with-1recipient-1solution-1additionalservice-0associatedservice-funding-complete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithServiceRecipientViewed(true)
                    .WithCatalogueSolutionsViewed(true)
                    .WithAssociatedServicesViewed(false)
                    .WithAdditionalServiceViewed(true)
                    .WithServiceRecipientEntity(
                        ServiceRecipientBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Od1")
                            .WithName("Service Recipient")
                            .Build())
                    .WithCatalogueSolutionEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods1")
                            .WithCatalogueItemName("Order Item 1")
                            .WithCatalogueItemType(CatalogueItemType.Solution)
                            .Build())
                    .WithAdditionalServicesEntity(
                        OrderItemEntityBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Ods2")
                        .WithCatalogueItemName("Order Item 2")
                        .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                        .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                },
                {"incomplete-with-0recipient-0solution-1associatedservice-funding-complete", async (orderId) => await OrderSummaryDataBuilder.Create(orderId)
                    .WithServiceRecipientViewed(false)
                    .WithCatalogueSolutionsViewed(false)
                    .WithAdditionalServiceViewed(false)
                    .WithAssociatedServicesViewed(true)
                    .WithAssociatedServicesEntity(
                        OrderItemEntityBuilder.Create()
                            .WithOrderId(orderId)
                            .WithOdsCode("Ods2")
                            .WithCatalogueItemName("Order Item 2 ")
                            .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                            .Build())
                    .Build()
                    .InsertAsync(_setting.ConnectionString)
                }
            };
    }
}
