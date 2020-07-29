using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class OrderSummaryDataBuilder
    {
        private List<OrderEntity> _OrderEntities;
        private List<OrderItemEntity> _OrderItemEntities;
        private List<ServiceRecipientEntity> _serviceRecipientEntites;

        private OrderSummaryDataBuilder( )
        {
            _OrderEntities = new List<OrderEntity>();
            _OrderItemEntities = new List<OrderItemEntity>();
            _serviceRecipientEntites = new List<ServiceRecipientEntity>();
        }

        public static OrderSummaryDataBuilder CreateOrderSummaryData(string orderId = "C000016-01") => 
            new OrderSummaryDataBuilder()
                .AddOrderEntity(
                    OrderEntityBuilder.Create()
                        .WithOrderStatusId(2)
                        .WithOrderId(orderId)
                        .WithDescription("A Description")
                        .WithOrganisationId(new Guid("4af62b99-638c-4247-875e-965239cd0c48"))
                        .WithServiceRecipientsViewed(true)
                        .WithAdditionalServicesViewed(true)
                        .WithCatalogueSolutionsViewed(true)
                        .WithAssociatedServicesViewed(true)
                        .WithFundingSourceOnlyGMS(true)
                        .Build())
                .AddServiceRecipientEntity(
                    ServiceRecipientBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Od1")
                        .WithName("Service Recipient")
                        .Build())
                .AddOrderItemEntity(
                    OrderItemEntityBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Ods1")
                        .WithCatalogueItemName("Order Item 1 ")
                        .WithCatalogueItemType(CatalogueItemType.Solution)
                        .Build())
                .AddOrderItemEntity(
                    OrderItemEntityBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Ods2")
                        .WithCatalogueItemName("Order Item 2 ")
                        .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                        .Build())
                .AddOrderItemEntity(
                    OrderItemEntityBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Ods3")
                        .WithCatalogueItemName("Order Item 3 ")
                        .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                        .Build());

        public OrderSummaryDataBuilder RemoveOrders()
        {
            _OrderEntities = new List<OrderEntity>();
            return this;
        }

        public OrderSummaryDataBuilder AddOrderEntity(OrderEntity orders)
        {
            _OrderEntities.Add(orders);
            return this;
        }

        public OrderSummaryDataBuilder AddServiceRecipientEntity(ServiceRecipientEntity serviceRecipient)
        {
            _serviceRecipientEntites.Add(serviceRecipient);
            return this;
        }

        public OrderSummaryDataBuilder RemoveServiceRecipients()
        {
            _serviceRecipientEntites = new List<ServiceRecipientEntity>();
            return this;
        }

        public OrderSummaryDataBuilder WithOrderItemEntityList(List<OrderItemEntity> orderItems)
        {
            _OrderItemEntities = orderItems ?? new List<OrderItemEntity>();
            return this;
        }

        public OrderSummaryDataBuilder RemoveAssociatedServices()
        {
            _OrderItemEntities.RemoveAll(oi => oi.CatalogueItemType == CatalogueItemType.AssociatedService);
            return this;
        }

        public OrderSummaryDataBuilder RemoveAdditionalServices()
        {
            _OrderItemEntities.RemoveAll(oi => oi.CatalogueItemType == CatalogueItemType.AdditionalService);
            return this;
        }

        public OrderSummaryDataBuilder RemoveSolutionsServices()
        {
            _OrderItemEntities.RemoveAll(oi => oi.CatalogueItemType == CatalogueItemType.Solution);
            return this;
        }

        public OrderSummaryDataBuilder AddOrderItemEntity(OrderItemEntity orderItem)
        {
            _OrderItemEntities.Add(orderItem);
            return this;
        }

        public OrderSummaryDataBuilder Build()
        {
            return this;
        }

        public async Task InsertAsync(string connectionString)
        {
            foreach (var order in _OrderEntities)
            {
                await order.InsertAsync(connectionString);
            }

            foreach (var recipients in _serviceRecipientEntites)
            {
                await recipients.InsertAsync(connectionString);
            }

            foreach (var orderItem in _OrderItemEntities)
            {
                await orderItem.InsertAsync(connectionString);
            }
        }
    }
}
