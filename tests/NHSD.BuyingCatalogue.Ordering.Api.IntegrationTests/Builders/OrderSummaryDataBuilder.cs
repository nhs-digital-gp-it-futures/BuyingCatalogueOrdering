using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class OrderSummaryDataBuilder
    {
        private OrderEntity _orderEntity;
        private ServiceRecipientEntity _serviceRecipientEntity;
        private OrderItemEntity _catalogueSolutionEntity;
        private OrderItemEntity _associatedServiceEntity;
        private OrderItemEntity _additionalServiceEntity;
        private bool _serviceRecipientsViewed = true;
        private bool _additionalServicesViewed = true;
        private bool _associatedServicesViewed = true;
        private bool _catalogueSolutionsViewed = true;
        private bool? _fundingSource = true;

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

        public static OrderSummaryDataBuilder Create(string orderId="C000016-01") =>
            new OrderSummaryDataBuilder(orderId);

        public OrderSummaryDataBuilder WithOrderEntity(OrderEntity orderEntity)
        {
            _orderEntity = orderEntity;
            return this;
        }

        public OrderSummaryDataBuilder WithServiceRecipientViewed(bool viewed)
        {
            _serviceRecipientsViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithAdditionalServiceViewed(bool viewed)
        {
            _additionalServicesViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithAssociatedServicesViewed(bool viewed)
        {
            _associatedServicesViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithCatalogueSolutionsViewed(bool viewed)
        {
            _catalogueSolutionsViewed = viewed;
            return this;
        }

        public OrderSummaryDataBuilder WithFundingSourceOnlyGMS(bool? funded)
        {
            _fundingSource = funded;
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
            if (_orderEntity != null)
            {
                _orderEntity.ServiceRecipientsViewed = _serviceRecipientsViewed;
                _orderEntity.AdditionalServicesViewed = _additionalServicesViewed;
                _orderEntity.AssociatedServicesViewed = _associatedServicesViewed;
                _orderEntity.CatalogueSolutionsViewed = _catalogueSolutionsViewed;
                _orderEntity.FundingSourceOnlyGMS = _fundingSource;
            }

            return new OrderSummaryData(_orderEntity, _serviceRecipientEntity, _catalogueSolutionEntity, _additionalServiceEntity, _associatedServiceEntity);
        }
    }
}
