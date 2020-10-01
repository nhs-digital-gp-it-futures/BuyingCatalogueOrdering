using System;
using System.Collections.Generic;
using System.Reflection;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    public sealed class OrderBuilder
    {
        private readonly string organisationName;
        private readonly string organisationOdsCode;
        private readonly Address organisationAddress;
        private readonly DateTime created;
        private readonly IList<OrderItem> orderItems = new List<OrderItem>();
        private readonly IList<OdsOrganisation> serviceRecipients = new List<OdsOrganisation>();
        private string orderId;
        private string orderDescription;
        private Guid organisationId;
        private Contact organisationContact;
        private DateTime lastUpdated;
        private Guid lastUpdatedBy;
        private string lastUpdatedByName;
        private string supplierId;
        private string supplierName;
        private Address supplierAddress;
        private Contact supplierContact;
        private DateTime? commencementDate;
        private bool serviceRecipientsViewed;
        private bool catalogueSolutionsViewed;
        private bool additionalServicesViewed;
        private bool associatedServicesViewed;
        private bool? fundingSourceOnlyGms;
        private DateTime? completed;

        private OrderBuilder()
        {
            orderId = "C000014-01";
            orderDescription = "Some Description";
            organisationId = Guid.NewGuid();
            organisationName = "Organisation Name";
            organisationOdsCode = "Ods Code";
            organisationAddress = AddressBuilder.Create().WithLine1("1 Some Ordering Party").Build();
            organisationContact = null;
            created = DateTime.UtcNow;
            lastUpdated = DateTime.UtcNow;
            lastUpdatedBy = Guid.NewGuid();
            lastUpdatedByName = "Bob Smith";
            supplierId = "Some supplier id";
            supplierName = "Some supplier name";
            supplierAddress = AddressBuilder.Create().WithLine1("1 Some Supplier").Build();
            supplierContact = null;
            commencementDate = null;
            serviceRecipientsViewed = false;
            catalogueSolutionsViewed = false;
            additionalServicesViewed = false;
            associatedServicesViewed = false;
            fundingSourceOnlyGms = null;
            completed = null;
        }

        public static OrderBuilder Create() => new OrderBuilder();

        public OrderBuilder WithOrderId(string orderId)
        {
            this.orderId = orderId;
            return this;
        }

        public OrderBuilder WithDescription(string description)
        {
            orderDescription = description;
            return this;
        }

        public OrderBuilder WithOrganisationId(Guid organisationId)
        {
            this.organisationId = organisationId;
            return this;
        }

        public OrderBuilder WithOrganisationContact(Contact organisationContact)
        {
            this.organisationContact = organisationContact;
            return this;
        }

        public OrderBuilder WithSupplierId(string supplierId)
        {
            this.supplierId = supplierId;
            return this;
        }

        public OrderBuilder WithSupplierName(string supplierName)
        {
            this.supplierName = supplierName;
            return this;
        }

        public OrderBuilder WithSupplierAddress(Address supplierAddress)
        {
            this.supplierAddress = supplierAddress;
            return this;
        }

        public OrderBuilder WithSupplierContact(Contact supplierContact)
        {
            this.supplierContact = supplierContact;
            return this;
        }

        public OrderBuilder WithCommencementDate(DateTime? commencementDate)
        {
            this.commencementDate = commencementDate;
            return this;
        }

        public OrderBuilder WithServiceRecipientsViewed(bool serviceRecipientsViewed)
        {
            this.serviceRecipientsViewed = serviceRecipientsViewed;
            return this;
        }

        public OrderBuilder WithCatalogueSolutionsViewed(bool catalogueSolutionsViewed)
        {
            this.catalogueSolutionsViewed = catalogueSolutionsViewed;
            return this;
        }

        public OrderBuilder WithAdditionalServicesViewed(bool additionalServicesViewed)
        {
            this.additionalServicesViewed = additionalServicesViewed;
            return this;
        }

        public OrderBuilder WithAssociatedServicesViewed(bool associatedServicesViewed)
        {
            this.associatedServicesViewed = associatedServicesViewed;
            return this;
        }

        public OrderBuilder WithFundingSourceOnlyGms(bool? fundingSourceOnlyGms)
        {
            this.fundingSourceOnlyGms = fundingSourceOnlyGms;
            return this;
        }

        public OrderBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            this.lastUpdatedBy = lastUpdatedBy;
            return this;
        }

        public OrderBuilder WithLastUpdatedByName(string lastUpdatedByName)
        {
            this.lastUpdatedByName = lastUpdatedByName;
            return this;
        }

        public OrderBuilder WithLastUpdated(DateTime lastUpdated)
        {
            this.lastUpdated = lastUpdated;
            return this;
        }

        public OrderBuilder WithCompleted(DateTime? completed)
        {
            this.completed = completed;
            return this;
        }

        public OrderBuilder WithOrderItem(OrderItem orderItem)
        {
            orderItems.Add(orderItem);
            return this;
        }

        public OrderBuilder WithServiceRecipient(string code, string name)
        {
            return WithServiceRecipient(new OdsOrganisation(code, name));
        }

        public OrderBuilder WithServiceRecipient(OdsOrganisation serviceRecipient)
        {
            serviceRecipients.Add(serviceRecipient);
            return this;
        }

        public Order Build()
        {
            var order = Order.Create(OrderDescription.Create(orderDescription).Value, organisationId, lastUpdatedBy, lastUpdatedByName);

            order.OrderId = orderId;
            order.OrganisationName = organisationName;
            order.OrganisationOdsCode = organisationOdsCode;
            order.OrganisationAddress = organisationAddress;
            order.OrganisationContact = organisationContact;
            order.SupplierId = supplierId;
            order.SupplierName = supplierName;
            order.SupplierAddress = supplierAddress;
            order.SupplierContact = supplierContact;
            order.CommencementDate = commencementDate;

            foreach (var orderItem in orderItems)
            {
                order.AddOrderItem(orderItem, lastUpdatedBy, lastUpdatedByName);
            }

            order.SetServiceRecipients(
                serviceRecipients,
                lastUpdatedBy,
                lastUpdatedByName);

            order.AdditionalServicesViewed = additionalServicesViewed;
            order.ServiceRecipientsViewed = serviceRecipientsViewed;
            order.CatalogueSolutionsViewed = catalogueSolutionsViewed;
            order.AssociatedServicesViewed = associatedServicesViewed;
            order.FundingSourceOnlyGMS = fundingSourceOnlyGms;
            order.Created = created;
            order.LastUpdated = lastUpdated;

            if (completed != null)
            {
                var completedFieldInfo =
                    order.GetType().GetField("completed", BindingFlags.Instance | BindingFlags.NonPublic);
                completedFieldInfo?.SetValue(order, completed);
            }

            return order;
        }
    }
}
