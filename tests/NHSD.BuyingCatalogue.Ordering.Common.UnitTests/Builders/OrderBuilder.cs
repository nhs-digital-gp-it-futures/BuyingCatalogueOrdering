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
        private readonly string orderDescription;
        private readonly Guid organisationId;
        private readonly Contact organisationContact;
        private readonly DateTime lastUpdated;
        private readonly string supplierId;
        private readonly string supplierName;
        private readonly Address supplierAddress;
        private readonly DateTime? completed;

        private int orderId;
        private Contact supplierContact;
        private DateTime? commencementDate;
        private bool serviceRecipientsViewed;
        private bool catalogueSolutionsViewed;
        private bool additionalServicesViewed;
        private bool associatedServicesViewed;
        private bool? fundingSourceOnlyGms;

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

        public OrderBuilder WithOrderItem(OrderItem orderItem)
        {
            orderItems.Add(orderItem);
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
