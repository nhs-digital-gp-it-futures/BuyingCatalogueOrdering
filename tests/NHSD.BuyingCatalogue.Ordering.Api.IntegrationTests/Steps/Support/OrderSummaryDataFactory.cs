﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    internal sealed class OrderSummaryDataFactory
    {
        private readonly Settings setting;

        internal OrderSummaryDataFactory(Settings setting)
        {
            this.setting = setting;
        }

        public IDictionary<string, Func<string, Task>> DataFactory =>
            new Dictionary<string, Func<string, Task>>
            {
                {
                    "complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
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
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "complete-with-1recipient-1associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
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
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "complete-with-0recipient-1associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
                        .WithAssociatedServicesEntity(
                            OrderItemEntityBuilder.Create()
                                .WithOrderId(orderId)
                                .WithOdsCode("Ods2")
                                .WithCatalogueItemName("Order Item 2 ")
                                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                                .Build())
                        .Build()
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "complete-with-1solution-1associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
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
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "complete-with-1solution-0associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
                        .WithCatalogueSolutionEntity(
                            OrderItemEntityBuilder.Create()
                                .WithOrderId(orderId)
                                .WithOdsCode("Ods1")
                                .WithCatalogueItemName("Order Item 1 ")
                                .WithCatalogueItemType(CatalogueItemType.Solution)
                                .Build())
                        .Build()
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "funding-incomplete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
                        .WithFundingSourceOnlyGMS(null)
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
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "incomplete-with-0recipient-0solution-0associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
                        .WithServiceRecipientViewed(false)
                        .WithCatalogueSolutionsViewed(false)
                        .WithAssociatedServicesViewed(false)
                        .WithAdditionalServiceViewed(false)
                        .Build()
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "incomplete-with-1recipient-0solution-0associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
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
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "incomplete-with-1recipient-1solution-0associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
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
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "incomplete-with-1recipient-1solution-1additionalservice-0associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
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
                        .InsertAsync(setting.ConnectionString)
                },
                {
                    "incomplete-with-0recipient-0solution-1associatedservice-funding-complete", async orderId => await OrderSummaryDataBuilder.Create(orderId)
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
                        .InsertAsync(setting.ConnectionString)
                },
            };

        public async Task CreateData(string key, string orderId)
        {
            await DataFactory[key](orderId);
        }
    }
}
