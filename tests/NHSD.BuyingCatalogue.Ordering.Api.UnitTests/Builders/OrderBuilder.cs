using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderBuilder
    {
        private readonly Order _order;

        private OrderBuilder()
        {
            _order = new Order
            {
                OrderId = "C000014-01",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Organisation Name",
                OrganisationOdsCode = "Ods Code",
                OrganisationAddress = AddressBuilder.Create().WithLine1("1 Some Ordering Party").Build(),
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                LastUpdatedBy = Guid.NewGuid(),
                LastUpdatedByName = "Bob Smith",
                OrderStatus = new OrderStatus() { OrderStatusId = 1, Name = "Submitted" },
                SupplierId = "Some supplier id",
                SupplierName = "Some supplier name",
                SupplierAddress = AddressBuilder.Create().WithLine1("1 Some Supplier").Build()
            };
            _order.SetDescription(OrderDescription.Create("Some Description").Value);
        }

        internal static OrderBuilder Create() => new OrderBuilder();

        internal OrderBuilder WithOrderId(string orderId)
        {
            _order.OrderId = orderId;
            return this;
        }

        internal OrderBuilder WithDescription(string description)
        {
            _order.SetDescription(OrderDescription.Create(description).Value);
            return this;
        }

        internal OrderBuilder WithOrganisationId(Guid organisationId)
        {
            _order.OrganisationId = organisationId;
            return this;
        }

        internal OrderBuilder WithCreated(DateTime created)
        {
            _order.Created = created;
            return this;
        }

        internal OrderBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _order.LastUpdated = lastUpdated;
            return this;
        }

        internal OrderBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _order.LastUpdatedBy = lastUpdatedBy;
            return this;
        }

        internal OrderBuilder WithLastUpdatedBy(string lastUpdatedByName)
        {
            _order.LastUpdatedByName = lastUpdatedByName;
            return this;
        }

        internal OrderBuilder WithOrderStatus(OrderStatus orderStatus)
        {
            _order.OrderStatus = orderStatus;
            return this;
        }

        internal OrderBuilder WithOrganisationContact(Contact organisationContact)
        {
            _order.OrganisationContact = organisationContact;
            return this;
        }

        internal OrderBuilder WithSupplierId(string supplierId)
        {
            _order.SupplierId = supplierId;
            return this;
        }

        internal OrderBuilder WithSupplierName(string supplierName)
        {
            _order.SupplierName = supplierName;
            return this;
        }

        internal OrderBuilder WithSupplierAddress(Address supplierAddress)
        {
            _order.SupplierAddress = supplierAddress;
            return this;
        }

        internal OrderBuilder WithSupplierContact(Contact supplierContact)
        {
            _order.SupplierContact = supplierContact;
            return this;
        }

        internal OrderBuilder WithCommencementDate(DateTime? commencementDate)
        {
            _order.CommencementDate = commencementDate;
            return this;
        }

        internal OrderBuilder WithServiceRecipientsViewed(bool serviceRecipientsViewed)
        {
            _order.ServiceRecipientsViewed = serviceRecipientsViewed;
            return this;
        }

        internal Order Build() => _order;
    }
}
