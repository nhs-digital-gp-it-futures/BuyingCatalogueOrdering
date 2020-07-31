using System;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class OrderEntityBuilder
    {
        private string _orderId;
        private string _description;
        private Guid   _organisationId;
        private string _organisationName;
        private string _organisationOdsCode;
        private int? _organisationAddressId;
        private int? _organisationBillingAddressId;
        private int? _organisationContactId;
        private OrderStatus _orderStatus;
        private DateTime _lastUpdated;
        private Guid _lastUpdatedBy;
        private string _lastUpdatedByName;
        private DateTime _created;
        private string _supplierId;
        private string _supplierName;
        private int? _supplierAddressId;
        private int? _supplierContactId;
        private DateTime? _commencementDate;
        private bool _serviceRecipientsViewed;
        private bool _catalogueSolutionsViewed;
        private bool _additionalServicesViewed;
        private bool _associatedServicesViewed;
        private bool? _fundingSourceOnlyGms;

        private OrderEntityBuilder()
        {
            _orderId = null;
            _description = "Some Description";
            _organisationId = Guid.NewGuid();
            _organisationName = null;
            _organisationOdsCode = "test";
            _organisationAddressId = null;
            _organisationBillingAddressId = null;
            _organisationContactId = null;
            _orderStatus = 0;
            _lastUpdated = DateTime.UtcNow;
            _lastUpdatedBy = Guid.NewGuid();
            _lastUpdatedByName = "Alice Smith";
            _created = DateTime.UtcNow;
            _supplierId = null;
            _supplierName = null;
            _supplierAddressId = null;
            _supplierContactId = null;
            _commencementDate = null;
            _serviceRecipientsViewed = false;
            _catalogueSolutionsViewed = false;
            _additionalServicesViewed = false;
            _fundingSourceOnlyGms = null;
        }

        public static OrderEntityBuilder Create()
        {
            return new OrderEntityBuilder();
        }

        public OrderEntityBuilder WithOrderId(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        public OrderEntityBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public OrderEntityBuilder WithOrganisationId(Guid organisationId)
        {
            _organisationId = organisationId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationName(string organisationName)
        {
            _organisationName = organisationName;
            return this;
        }

        public OrderEntityBuilder WithOrganisationOdsCode(string odsCode)
        {
            _organisationOdsCode = odsCode;
            return this;
        }

        public OrderEntityBuilder WithOrganisationAddressId(int? addressId)
        {
            _organisationAddressId = addressId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationBillingAddressId(int? billingAddressId)
        {
            _organisationBillingAddressId = billingAddressId;
            return this;
        }

        public OrderEntityBuilder WithOrganisationContactId(int? organisationContactId)
        {
            _organisationContactId = organisationContactId;
            return this;
        }

        public OrderEntityBuilder WithOrderStatus(OrderStatus orderStatus)
        {
            _orderStatus = orderStatus;
            return this;
        }

        public OrderEntityBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _lastUpdated = lastUpdated;
            return this;
        }

        public OrderEntityBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _lastUpdatedBy = lastUpdatedBy;
            return this;
        }

        public OrderEntityBuilder WithLastUpdatedName(string lastUpdatedByName)
        {
            _lastUpdatedByName = lastUpdatedByName;
            return this;
        }

        public OrderEntityBuilder WithDateCreated(DateTime dateCreated)
        {
            _created = dateCreated;
            return this;
        }

        public OrderEntityBuilder WithSupplierId(string supplierId)
        {
            _supplierId = supplierId;
            return this;
        }

        public OrderEntityBuilder WithSupplierName(string supplierName)
        {
            _supplierName = supplierName;
            return this;
        }

        public OrderEntityBuilder WithSupplierAddressId(int? supplierAddressId)
        {
            _supplierAddressId = supplierAddressId;
            return this;
        }

        public OrderEntityBuilder WithSupplierContactId(int? supplierContactId)
        {
            _supplierContactId = supplierContactId;
            return this;
        }

        public OrderEntityBuilder WithCommencementDate(DateTime? commencementDate)
        {
            _commencementDate = commencementDate;
            return this;
        }

        public OrderEntityBuilder WithServiceRecipientsViewed(bool serviceRecipientsViewed)
        {
            _serviceRecipientsViewed = serviceRecipientsViewed;
            return this;
        }

        public OrderEntityBuilder WithCatalogueSolutionsViewed(bool catalogueSolutionsViewed)
        {
            _catalogueSolutionsViewed = catalogueSolutionsViewed;
            return this;
        }

        public OrderEntityBuilder WithAdditionalServicesViewed(bool additionalServicesViewed)
        {
            _additionalServicesViewed = additionalServicesViewed;
            return this;
        }

        public OrderEntityBuilder WithAssociatedServicesViewed(bool associatedServicesViewed)
        {
            _associatedServicesViewed = associatedServicesViewed;
            return this;
        }

        public OrderEntityBuilder WithFundingSourceOnlyGms(bool? fundingSourceOnlyGms)
        {
            _fundingSourceOnlyGms = fundingSourceOnlyGms;
            return this;
        }

        public OrderEntity Build()
        {
            return new OrderEntity
            {
                OrderId = _orderId,
                Description = _description,
                OrganisationId = _organisationId,
                OrganisationName = _organisationName,
                OrganisationOdsCode = _organisationOdsCode,
                OrganisationAddressId = _organisationAddressId,
                OrganisationBillingAddressId = _organisationBillingAddressId,
                OrganisationContactId = _organisationContactId,
                OrderStatus = _orderStatus,
                LastUpdated = _lastUpdated,
                LastUpdatedBy = _lastUpdatedBy,
                LastUpdatedByName = _lastUpdatedByName,
                Created = _created,
                SupplierId = _supplierId,
                SupplierName = _supplierName,
                SupplierAddressId = _supplierAddressId,
                SupplierContactId = _supplierContactId,
                CommencementDate = _commencementDate,
                ServiceRecipientsViewed = _serviceRecipientsViewed,
                CatalogueSolutionsViewed = _catalogueSolutionsViewed,
                AdditionalServicesViewed = _additionalServicesViewed,
                AssociatedServicesViewed = _associatedServicesViewed,
                FundingSourceOnlyGMS = _fundingSourceOnlyGms
            };
        }
    }
}
