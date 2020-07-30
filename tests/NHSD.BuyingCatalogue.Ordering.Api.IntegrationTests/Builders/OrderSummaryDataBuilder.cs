using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using CatalogueItemType = NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data.CatalogueItemType;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class OrderSummaryDataBuilder
    {
        private List<OrderEntity> _orderEntities = new List<OrderEntity>();
        private List<OrderItemEntity> _associatedServiceEntities = new List<OrderItemEntity>();
        private List<OrderItemEntity> _catalogueSolutionEntities = new List<OrderItemEntity>();
        private List<OrderItemEntity> _additionalServiceEntities = new List<OrderItemEntity>();
        private List<ServiceRecipientEntity> _serviceRecipientEntities = new List<ServiceRecipientEntity>();

        private OrderSummaryDataBuilder(string orderId="C000016-01")
        {
            AddOrderEntity(
                OrderEntityBuilder.Create()
                    .WithOrderStatusId((int)OrderStatus.Unsubmitted)
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
                .AddCatalogSolutionEntity(
                    OrderItemEntityBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Ods1")
                        .WithCatalogueItemName("Order Item 1 ")
                        .WithCatalogueItemType(CatalogueItemType.Solution)
                        .Build())
                .AddAssociatedServiceEntity(
                    OrderItemEntityBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Ods2")
                        .WithCatalogueItemName("Order Item 2 ")
                        .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                        .Build())
                .AddAdditionalServiceEntity(
                    OrderItemEntityBuilder.Create()
                        .WithOrderId(orderId)
                        .WithOdsCode("Ods3")
                        .WithCatalogueItemName("Order Item 3 ")
                        .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                        .Build());
        }

        public static OrderSummaryDataBuilder Create(string orderId="C000016-01") =>
            new OrderSummaryDataBuilder(orderId);


        public OrderSummaryDataBuilder WithOrderEntities(List<OrderEntity> orderEntities)
        {
            _orderEntities = orderEntities ?? new List<OrderEntity>();
            return this;
        }

        public OrderSummaryDataBuilder AddOrderEntity(OrderEntity orders)
        {
            _orderEntities.Add(orders);
            return this;
        }

        public OrderSummaryDataBuilder AddServiceRecipientEntity(ServiceRecipientEntity serviceRecipient)
        {
            _serviceRecipientEntities.Add(serviceRecipient);
            return this;
        }

        public OrderSummaryDataBuilder WithServiceRecipientEntities(List<ServiceRecipientEntity> serviceRecipientEntities)
        {
            _serviceRecipientEntities = serviceRecipientEntities ?? new List<ServiceRecipientEntity>();
            return this;
        }

        public OrderSummaryDataBuilder WithAdditionalServicesEntities(List<OrderItemEntity> additionalServiceEntities)
        {
            _additionalServiceEntities = additionalServiceEntities ?? new List<OrderItemEntity>();
            return this;
        }

        public OrderSummaryDataBuilder  AddAdditionalServiceEntity(OrderItemEntity additionalServiceEntity)
        {
            if (additionalServiceEntity.CatalogueItemType == CatalogueItemType.AdditionalService)
            {
                _additionalServiceEntities.Add(additionalServiceEntity);
            }
            return this;
        }

        public OrderSummaryDataBuilder WithAssociatedServicesEntities(List<OrderItemEntity> associatedServicesEntities)
        {
            _associatedServiceEntities = associatedServicesEntities ?? new List<OrderItemEntity>();
            return this;
        }

        public OrderSummaryDataBuilder AddAssociatedServiceEntity(OrderItemEntity associatedServiceEntity)
        {
            if (associatedServiceEntity.CatalogueItemType == CatalogueItemType.AssociatedService)
            {
                _associatedServiceEntities.Add(associatedServiceEntity);
            }
            return this;
        }

        public OrderSummaryDataBuilder WithCatalogueSolutionEntities(List<OrderItemEntity> catalogueSolutionEntities)
        {
            _associatedServiceEntities = catalogueSolutionEntities ?? new List<OrderItemEntity>();
            return this;
        }

        public OrderSummaryDataBuilder AddCatalogSolutionEntity(OrderItemEntity catalogueSolutionEntity)
        {
            if (catalogueSolutionEntity.CatalogueItemType == CatalogueItemType.Solution)
            {
                _catalogueSolutionEntities.Add(catalogueSolutionEntity);
            }
            return this;
        }

        public OrderSummaryData Build()
        {
            IEnumerable<OrderItemEntity> orderItems = _additionalServiceEntities.Union(_associatedServiceEntities).Union(_catalogueSolutionEntities);
            return new OrderSummaryData(_orderEntities, orderItems.ToList(), _serviceRecipientEntities);
        }
    }
}
