using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class OrderEntityBuilder
    {
        private string orderId;
        private string description;
        private Guid organisationId;
        private string organisationName;
        private string organisationOdsCode;
        private int? organisationAddressId;
        private int? organisationBillingAddressId;
        private int? organisationContactId;
        private OrderStatus orderStatus;
        private DateTime lastUpdated;
        private Guid lastUpdatedBy;
        private string lastUpdatedByName;
        private DateTime created;
        private DateTime? completed;
        private string supplierId;
        private string supplierName;
        private int? supplierAddressId;
        private int? supplierContactId;
        private DateTime? commencementDate;
        private bool serviceRecipientsViewed;
        private bool catalogueSolutionsViewed;
        private bool additionalServicesViewed;
        private bool associatedServicesViewed;
        private bool? fundingSourceOnlyGms;
        private bool isDeleted;

        private OrderEntityBuilder()
        {
            orderId = null;
            description = "Some Description";
            organisationId = Guid.NewGuid();
            organisationName = null;
            organisationOdsCode = "test";
            organisationAddressId = null;
            organisationBillingAddressId = null;
            organisationContactId = null;
            orderStatus = 0;
            lastUpdated = DateTime.UtcNow;
            lastUpdatedBy = Guid.NewGuid();
            lastUpdatedByName = "Alice Smith";
            created = DateTime.UtcNow;
            supplierId = null;
            supplierName = null;
            supplierAddressId = null;
            supplierContactId = null;
            commencementDate = null;
            serviceRecipientsViewed = false;
            catalogueSolutionsViewed = false;
            additionalServicesViewed = false;
            fundingSourceOnlyGms = null;
            isDeleted = false;
        }

        public static OrderEntityBuilder Create()
        {
            return new();
        }

        public OrderEntityBuilder WithOrderId(string id)
        {
            orderId = id;
            return this;
        }

        public OrderEntityBuilder WithDescription(string value)
        {
            description = value;
            return this;
        }

        public OrderEntityBuilder WithOrganisationId(Guid id)
        {
            organisationId = id;
            return this;
        }

        public OrderEntityBuilder WithOrganisationName(string name)
        {
            organisationName = name;
            return this;
        }

        public OrderEntityBuilder WithOrganisationOdsCode(string odsCode)
        {
            organisationOdsCode = odsCode;
            return this;
        }

        public OrderEntityBuilder WithOrganisationAddressId(int? addressId)
        {
            organisationAddressId = addressId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationBillingAddressId(int? billingAddressId)
        {
            organisationBillingAddressId = billingAddressId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationContactId(int? contactId)
        {
            organisationContactId = contactId;
            return this;
        }

        public OrderEntityBuilder WithOrderStatus(OrderStatus status)
        {
            orderStatus = status;
            return this;
        }

        public OrderEntityBuilder WithLastUpdated(DateTime date)
        {
            lastUpdated = date;
            return this;
        }

        public OrderEntityBuilder WithLastUpdatedBy(Guid userId)
        {
            lastUpdatedBy = userId;
            return this;
        }

        public OrderEntityBuilder WithLastUpdatedName(string userName)
        {
            lastUpdatedByName = userName;
            return this;
        }

        public OrderEntityBuilder WithDateCreated(DateTime dateCreated)
        {
            created = dateCreated;
            return this;
        }

        public OrderEntityBuilder WithSupplierId(string id)
        {
            supplierId = id;
            return this;
        }

        public OrderEntityBuilder WithSupplierName(string name)
        {
            supplierName = name;
            return this;
        }

        public OrderEntityBuilder WithSupplierAddressId(int? addressId)
        {
            supplierAddressId = addressId;
            return this;
        }

        public OrderEntityBuilder WithSupplierContactId(int? contactId)
        {
            supplierContactId = contactId;
            return this;
        }

        public OrderEntityBuilder WithCommencementDate(DateTime? date)
        {
            commencementDate = date;
            return this;
        }

        public OrderEntityBuilder WithServiceRecipientsViewed(bool viewed)
        {
            serviceRecipientsViewed = viewed;
            return this;
        }

        public OrderEntityBuilder WithCatalogueSolutionsViewed(bool viewed)
        {
            catalogueSolutionsViewed = viewed;
            return this;
        }

        public OrderEntityBuilder WithAdditionalServicesViewed(bool viewed)
        {
            additionalServicesViewed = viewed;
            return this;
        }

        public OrderEntityBuilder WithAssociatedServicesViewed(bool viewed)
        {
            associatedServicesViewed = viewed;
            return this;
        }

        public OrderEntityBuilder WithFundingSourceOnlyGms(bool? onlyGms)
        {
            fundingSourceOnlyGms = onlyGms;
            return this;
        }

        public OrderEntityBuilder WithIsDeleted(bool deleted)
        {
            isDeleted = deleted;
            return this;
        }

        public OrderEntityBuilder WithDateCompleted(DateTime? dateCompleted)
        {
            completed = dateCompleted;
            return this;
        }

        public OrderEntity Build()
        {
            return new()
            {
                OrderId = orderId,
                Description = description,
                OrganisationId = organisationId,
                OrganisationName = organisationName,
                OrganisationOdsCode = organisationOdsCode,
                OrganisationAddressId = organisationAddressId,
                OrganisationBillingAddressId = organisationBillingAddressId,
                OrganisationContactId = organisationContactId,
                OrderStatus = orderStatus,
                LastUpdated = lastUpdated,
                LastUpdatedBy = lastUpdatedBy,
                LastUpdatedByName = lastUpdatedByName,
                Created = created,
                SupplierId = supplierId,
                SupplierName = supplierName,
                SupplierAddressId = supplierAddressId,
                SupplierContactId = supplierContactId,
                CommencementDate = commencementDate,
                ServiceRecipientsViewed = serviceRecipientsViewed,
                CatalogueSolutionsViewed = catalogueSolutionsViewed,
                AdditionalServicesViewed = additionalServicesViewed,
                AssociatedServicesViewed = associatedServicesViewed,
                FundingSourceOnlyGms = fundingSourceOnlyGms,
                IsDeleted = isDeleted,
                Completed = completed,
            };
        }
    }
}
