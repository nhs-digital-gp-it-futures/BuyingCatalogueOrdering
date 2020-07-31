using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class OrderSummaryDataBuilder
    {
        private OrderEntity _orderEntity;
        private ServiceRecipientEntity _serviceRecipientEntity;
        private OrderItemEntity _catalogueSolutionEntity;
        private OrderItemEntity _associatedServiceEntity;
        private OrderItemEntity _additionalServiceEntity;

        private OrderSummaryDataBuilder(string orderId="C000016-01")
        {
            WithOrderEntity(
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
                    .Build());
        }

        public static OrderSummaryDataBuilder Create(string orderId="C000016-01") =>
            new OrderSummaryDataBuilder(orderId);

        public OrderSummaryDataBuilder WithOrderEntity(OrderEntity orderEntity)
        {
            _orderEntity = orderEntity;
            return this;
        }

        public OrderSummaryDataBuilder WithServiceRecipientViewed(bool viewed)
        {
            if (_orderEntity != null)
            {
                _orderEntity.ServiceRecipientsViewed = viewed;
            }
            return this;
        }

        public OrderSummaryDataBuilder WithAdditionalServiceViewed(bool viewed)
        {
            if (_orderEntity != null)
            {
                _orderEntity.AdditionalServicesViewed = viewed;
            }
            return this;
        }

        public OrderSummaryDataBuilder WithAssociatedServicesViewed(bool viewed)
        {
            if (_orderEntity != null)
            {
                _orderEntity.AssociatedServicesViewed = viewed;
            }
            return this;
        }

        public OrderSummaryDataBuilder WithCatalogueSolutionsViewed(bool viewed)
        {
            if (_orderEntity != null)
            {
                _orderEntity.CatalogueSolutionsViewed = viewed;
            }
            return this;
        }

        public OrderSummaryDataBuilder WithFundingSourceOnlyGMS(bool? funded)
        {
            if (_orderEntity != null)
            {
                _orderEntity.FundingSourceOnlyGMS = funded;
            }
            return this;
        }

        public OrderSummaryDataBuilder WithCatalogueSolutionEntity(OrderItemEntity catalogueSolutionEntity)
        {
            _catalogueSolutionEntity = catalogueSolutionEntity;
            return this;
        }

        public OrderSummaryDataBuilder WithServiceRecipientEntity(ServiceRecipientEntity serviceRecipientEntity)
        {
            _serviceRecipientEntity = serviceRecipientEntity;
            return this;
        }

        public OrderSummaryDataBuilder WithAdditionalServicesEntity(OrderItemEntity additionalServiceEntity)
        {
            _additionalServiceEntity = additionalServiceEntity;
            return this;
        }

        public OrderSummaryDataBuilder WithAssociatedServicesEntity(OrderItemEntity associatedServicesEntity)
        {
            _associatedServiceEntity = associatedServicesEntity;
            return this;
        }

        public OrderSummaryData Build()
        {
            return new OrderSummaryData(_orderEntity, _serviceRecipientEntity, _catalogueSolutionEntity, _additionalServiceEntity, _associatedServiceEntity);
        }
    }
}
