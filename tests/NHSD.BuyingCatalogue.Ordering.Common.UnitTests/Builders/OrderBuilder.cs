using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    public sealed class OrderBuilder
    {
        private readonly string organisationName;
        private readonly string organisationOdsCode;
        private readonly Address organisationAddress;
        private readonly IList<OrderItem> orderItems = new List<OrderItem>();
        private readonly IList<ServiceInstanceItem> serviceInstanceItems = new List<ServiceInstanceItem>();
        private readonly List<SelectedServiceRecipient> serviceRecipients = new();
        private int orderId;
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
            orderId = 1;
            orderDescription = "Some Description";
            organisationId = Guid.NewGuid();

            organisationName = "Organisation Name";
            organisationOdsCode = "Ods Code";
            organisationAddress = AddressBuilder.Create().WithLine1("1 Some Ordering Party").Build();
            organisationContact = null;
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

        public OrderBuilder WithOrderId(int id)
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
            return WithServiceRecipient(new SelectedServiceRecipient { Recipient = new ServiceRecipient(code, name) });
        }

        public OrderBuilder WithServiceRecipient(SelectedServiceRecipient serviceRecipient)
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
                OrderItemId = 1, // forOrderItem.OrderItemId,
                ServiceInstanceId = $"SI{increment}-{forOrderItem.OrderItemRecipients[0].Recipient.OdsCode}",
            });

            return this;
        }

        public Order Build()
        {
            var order = new Order
            {
                CommencementDate = commencementDate,
                Description = orderDescription,
                FundingSourceOnlyGms = fundingSourceOnlyGms,
                OrderingParty = new OrderingParty
                {
                    Id = organisationId,
                    Name = organisationName,
                    OdsCode = organisationOdsCode,
                    Address = organisationAddress,
                },
                OrderingPartyContact = organisationContact,
                Progress = new OrderProgress
                {
                    AdditionalServicesViewed = additionalServicesViewed,
                    AssociatedServicesViewed = associatedServicesViewed,
                    CatalogueSolutionsViewed = catalogueSolutionsViewed,
                    ServiceRecipientsViewed = serviceRecipientsViewed,
                },
                Supplier = new Supplier
                {
                    Id = supplierId,
                    Name = supplierName,
                    Address = supplierAddress,
                },
                SupplierContact = supplierContact,
            };

            foreach (var orderItem in orderItems)
            {
                order.AddOrUpdateOrderItem(orderItem);
            }

            order.SetSelectedServiceRecipients(serviceRecipients);

            BackingField.SetValue(order, nameof(Order.LastUpdated), lastUpdated);
            BackingField.SetValue(order, nameof(Order.Id), orderId);
            BackingField.SetValue(order, nameof(Order.ServiceInstanceItems), serviceInstanceItems);

            if (completed is not null)
            {
                BackingField.SetValue(order, nameof(Order.Completed), completed);
            }

            return order;
        }
    }
}
