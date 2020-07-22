﻿using System;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class OrderEntityBuilder
    {
        private readonly OrderEntity _orderEntity;

        private OrderEntityBuilder()
        {
            _orderEntity = new OrderEntity
            {
                Description = "Some Description",
                OrganisationId = Guid.NewGuid(),
                OrganisationOdsCode = "test",
                OrganisationAddressId = null,
                OrganisationBillingAddressId = null,
                OrganisationContactId = null,
                OrderStatusId = 0,
                LastUpdated = DateTime.UtcNow,
                LastUpdatedBy = Guid.NewGuid(),
                LastUpdatedByName = "Alice Smith",
                Created = DateTime.UtcNow,
                SupplierId = null,
                SupplierName = null,
                CommencementDate = null,
                ServiceRecipientsViewed = false,
                CatalogueSolutionsViewed = false,
                AdditionalServicesViewed = false
            };
        }

        public static OrderEntityBuilder Create()
        {
            return new OrderEntityBuilder();
        }

        public OrderEntityBuilder WithOrderId(string orderId)
        {
            _orderEntity.OrderId = orderId;
            return this;
        }
        public OrderEntityBuilder WithDescription(string description)
        {
            _orderEntity.Description = description;
            return this;
        }

        public OrderEntityBuilder WithOrganisationId(Guid organisationId)
        {
            _orderEntity.OrganisationId = organisationId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationName(string organisationName)
        {
            _orderEntity.OrganisationName = organisationName;
            return this;
        }

        public OrderEntityBuilder WithOrganisationOdsCode(string odsCode)
        {
            _orderEntity.OrganisationOdsCode = odsCode;
            return this;
        }

        public OrderEntityBuilder WithOrganisationAddressId(int? addressId)
        {
            _orderEntity.OrganisationAddressId = addressId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationBillingAddressId(int? billingAddressId)
        {
            _orderEntity.OrganisationBillingAddressId = billingAddressId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationContactId(int? organisationContactId)
        {
            _orderEntity.OrganisationContactId = organisationContactId;
            return this;
        }

        public OrderEntityBuilder WithOrderStatusId(int statusId)
        {
            _orderEntity.OrderStatusId = statusId;
            return this;
        }

        public OrderEntityBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _orderEntity.LastUpdated = lastUpdated;
            return this;
        }

        public OrderEntityBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _orderEntity.LastUpdatedBy = lastUpdatedBy;
            return this;
        }

        public OrderEntityBuilder WithLastUpdatedName(string lastUpdatedByName)
        {
            _orderEntity.LastUpdatedByName = lastUpdatedByName;
            return this;
        }

        public OrderEntityBuilder WithDateCreated(DateTime dateCreated)
        {
            _orderEntity.Created = dateCreated;
            return this;
        }

        public OrderEntityBuilder WithSupplierId(string supplierId)
        {
            _orderEntity.SupplierId = supplierId;
            return this;
        }

        public OrderEntityBuilder WithSupplierName(string supplierName)
        {
            _orderEntity.SupplierName = supplierName;
            return this;
        }

        public OrderEntityBuilder WithSupplierAddressId(int? supplierAddressId)
        {
            _orderEntity.SupplierAddressId = supplierAddressId;
            return this;
        }

        public OrderEntityBuilder WithSupplierContactId(int? supplierContactId)
        {
            _orderEntity.SupplierContactId = supplierContactId;
            return this;
        }

        public OrderEntityBuilder WithCommencementDate(DateTime? commencementDate)
        {
            _orderEntity.CommencementDate = commencementDate;
            return this;
        }

        public OrderEntityBuilder WithServiceRecipientsViewed(bool serviceRecipientsViewed)
        {
            _orderEntity.ServiceRecipientsViewed = serviceRecipientsViewed;
            return this;
        }

        public OrderEntityBuilder WithCatalogueSolutionsViewed(bool catalogueSolutionsViewed)
        {
            _orderEntity.CatalogueSolutionsViewed = catalogueSolutionsViewed;
            return this;
        }

        public OrderEntityBuilder WithAdditionalServicesViewed(bool additionalServicesViewed)
        {
            _orderEntity.AdditionalServicesViewed = additionalServicesViewed;
            return this;
        }

        public OrderEntityBuilder WithAssociatedServicesViewed(bool associatedServicesViewed)
        {
            _orderEntity.AssociatedServicesViewed = associatedServicesViewed;
            return this;
        }

        public OrderEntity Build()
        {
            return _orderEntity;
        }
    }
}
