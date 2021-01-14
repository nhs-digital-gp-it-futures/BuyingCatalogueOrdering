using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class OrderSummaryDataBuilder
    {
        private OrderEntity orderEntity;
        private ServiceRecipientEntity serviceRecipientEntity;
        private OrderItemEntity catalogueSolutionEntity;
        private OrderItemEntity associatedServiceEntity;
        private OrderItemEntity additionalServiceEntity;
        private bool serviceRecipientsViewed = true;
        private bool additionalServicesViewed = true;
        private bool associatedServicesViewed = true;
        private bool catalogueSolutionsViewed = true;
        private bool? fundingSource = true;

        private OrderSummaryDataBuilder(string orderId = "C000016-01")
        {
            WithOrderEntity(
                OrderEntityBuilder.Create()
                    .WithOrderStatus(OrderStatus.Incomplete)
                    .WithOrderId(orderId)
                    .WithDescription("A Description")
                    .WithOrganisationId(new Guid("4af62b99-638c-4247-875e-965239cd0c48"))
                    .WithServiceRecipientsViewed(true)
                    .WithAdditionalServicesViewed(true)
                    .WithCatalogueSolutionsViewed(true)
                    .WithAssociatedServicesViewed(true)
                    .WithFundingSourceOnlyGms(true)
                    .Build());
        }

        public static OrderSummaryDataBuilder Create(string orderId = "C000016-01") => new(orderId);

        public OrderSummaryDataBuilder WithOrderEntity(OrderEntity entity)
        {
            orderEntity = entity;
            return this;
        }

        public OrderSummaryDataBuilder WithServiceRecipientViewed(bool viewed)
        {
            serviceRecipientsViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithAdditionalServiceViewed(bool viewed)
        {
            additionalServicesViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithAssociatedServicesViewed(bool viewed)
        {
            associatedServicesViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithCatalogueSolutionsViewed(bool viewed)
        {
            catalogueSolutionsViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithFundingSourceOnlyGMS(bool? funded)
        {
            fundingSource = funded;
            return this;
        }

        public OrderSummaryDataBuilder WithCatalogueSolutionEntity(OrderItemEntity entity)
        {
            catalogueSolutionEntity = entity;
            return this;
        }

        public OrderSummaryDataBuilder WithServiceRecipientEntity(ServiceRecipientEntity entity)
        {
            serviceRecipientEntity = entity;
            return this;
        }

        public OrderSummaryDataBuilder WithAdditionalServicesEntity(OrderItemEntity entity)
        {
            additionalServiceEntity = entity;
            return this;
        }

        public OrderSummaryDataBuilder WithAssociatedServicesEntity(OrderItemEntity entity)
        {
            associatedServiceEntity = entity;
            return this;
        }

        public OrderSummaryData Build()
        {
            if (orderEntity != null)
            {
                orderEntity.ServiceRecipientsViewed = serviceRecipientsViewed;
                orderEntity.AdditionalServicesViewed = additionalServicesViewed;
                orderEntity.AssociatedServicesViewed = associatedServicesViewed;
                orderEntity.CatalogueSolutionsViewed = catalogueSolutionsViewed;
                orderEntity.FundingSourceOnlyGms = fundingSource;
            }

            return new OrderSummaryData(orderEntity, serviceRecipientEntity, catalogueSolutionEntity, additionalServiceEntity, associatedServiceEntity);
        }
    }
}
