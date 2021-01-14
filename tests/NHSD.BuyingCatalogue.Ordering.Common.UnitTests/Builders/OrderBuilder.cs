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
        private readonly IList<ServiceInstanceItem> serviceInstanceItems = new List<ServiceInstanceItem>();
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

        public static OrderBuilder Create() => new();

        public OrderBuilder WithOrderId(string id)
        {
            orderId = id;
            return this;
        }

        public OrderBuilder WithDescription(string description)
        {
            orderDescription = description;
            return this;
        }

        public OrderBuilder WithOrganisationId(Guid id)
        {
            organisationId = id;
            return this;
        }

        public OrderBuilder WithOrganisationContact(Contact contact)
        {
            organisationContact = contact;
            return this;
        }

        public OrderBuilder WithSupplierId(string id)
        {
            supplierId = id;
            return this;
        }

        public OrderBuilder WithSupplierName(string name)
        {
            supplierName = name;
            return this;
        }

        public OrderBuilder WithSupplierAddress(Address address)
        {
            supplierAddress = address;
            return this;
        }

        public OrderBuilder WithSupplierContact(Contact contact)
        {
            supplierContact = contact;
            return this;
        }

        public OrderBuilder WithCommencementDate(DateTime? date)
        {
            commencementDate = date;
            return this;
        }

        public OrderBuilder WithServiceRecipientsViewed(bool viewed)
        {
            serviceRecipientsViewed = viewed;
            return this;
        }

        public OrderBuilder WithCatalogueSolutionsViewed(bool viewed)
        {
            catalogueSolutionsViewed = viewed;
            return this;
        }

        public OrderBuilder WithAdditionalServicesViewed(bool viewed)
        {
            additionalServicesViewed = viewed;
            return this;
        }

        public OrderBuilder WithAssociatedServicesViewed(bool viewed)
        {
            associatedServicesViewed = viewed;
            return this;
        }

        public OrderBuilder WithFundingSourceOnlyGms(bool? onlyGms)
        {
            fundingSourceOnlyGms = onlyGms;
            return this;
        }

        public OrderBuilder WithLastUpdatedBy(Guid updatedBy)
        {
            lastUpdatedBy = updatedBy;
            return this;
        }

        public OrderBuilder WithLastUpdatedByName(string name)
        {
            lastUpdatedByName = name;
            return this;
        }

        public OrderBuilder WithLastUpdated(DateTime updated)
        {
            lastUpdated = updated;
            return this;
        }

        public OrderBuilder WithCompleted(DateTime? dateCompleted)
        {
            completed = dateCompleted;
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

        public OrderBuilder WithServiceInstanceId(OrderItem forOrderItem, int increment)
        {
            if (forOrderItem is null)
                throw new ArgumentNullException(nameof(forOrderItem));

            serviceInstanceItems.Add(new ServiceInstanceItem
            {
                OrderItemId = forOrderItem.OrderItemId,
                ServiceInstanceId = $"SI{increment}-{forOrderItem.OdsCode}",
            });

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

            Field.Set(order, nameof(Order.ServiceInstanceItems), serviceInstanceItems);

            if (completed is not null)
            {
                Field.Set(order, nameof(Order.Completed), completed);
            }

            return order;
        }
    }
}
