using System;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class OrderEntityBuilder
    {
        private int orderId;
        private string description;
        private Guid organisationId;
        private int? organisationContactId;
        private OrderStatus orderStatus;
        private DateTime lastUpdated;
        private Guid lastUpdatedBy;
        private string lastUpdatedByName;
        private DateTime created;
        private DateTime? completed;
        private string supplierId;
        private int? supplierContactId;
        private DateTime? commencementDate;
        private bool? fundingSourceOnlyGms;
        private bool isDeleted;

        private OrderEntityBuilder()
        {
            description = "Some Description";
            organisationId = Guid.NewGuid();
            organisationContactId = null;
            orderStatus = 0;
            lastUpdated = DateTime.UtcNow;
            lastUpdatedBy = Guid.NewGuid();
            lastUpdatedByName = "Alice Smith";
            created = DateTime.UtcNow;
            supplierId = null;
            supplierContactId = null;
            commencementDate = null;
            fundingSourceOnlyGms = null;
            isDeleted = false;
        }

        public static OrderEntityBuilder Create()
        {
            return new();
        }

        public OrderEntityBuilder WithOrderId(int id)
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
                Id = orderId,
                Description = description,
                OrderingPartyId = organisationId,
                OrderingPartyContactId = organisationContactId,
                OrderStatus = orderStatus,
                LastUpdated = lastUpdated,
                LastUpdatedBy = lastUpdatedBy,
                LastUpdatedByName = lastUpdatedByName,
                Created = created,
                SupplierId = supplierId,
                SupplierContactId = supplierContactId,
                CommencementDate = commencementDate,
                FundingSourceOnlyGms = fundingSourceOnlyGms,
                IsDeleted = isDeleted,
                Completed = completed,
            };
        }
    }
}
