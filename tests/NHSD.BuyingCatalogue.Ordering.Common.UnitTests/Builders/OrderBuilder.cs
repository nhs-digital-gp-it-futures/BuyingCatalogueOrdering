using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders
{
    public sealed class OrderBuilder
    {
        private string _orderId;
        private string _orderDescription;
        private Guid _organisationId;
        private readonly string _organisationName;
        private readonly string _organisationOdsCode;
        private readonly Address _organisationAddress;
        private Contact _organisationContact;
        private readonly DateTime _created;
        private DateTime _lastUpdated;
        private Guid _lastUpdatedBy;
        private string _lastUpdatedByName;
        private string _supplierId;
        private string _supplierName;
        private Address _supplierAddress;
        private Contact _supplierContact;
        private DateTime? _commencementDate;
        private bool _serviceRecipientsViewed;
        private bool _catalogueSolutionsViewed;
        private bool _additionalServicesViewed;
        private readonly IList<OrderItem> _orderItems = new List<OrderItem>();
        private readonly IList<(string Ods, string Name)> _serviceRecipients = new List<(string Ods, string Name)>();

        private OrderBuilder()
        {
            _orderId = "C000014-01";
            _orderDescription = "Some Description";
            _organisationId = Guid.NewGuid();
            _organisationName = "Organisation Name";
            _organisationOdsCode = "Ods Code";
            _organisationAddress = AddressBuilder.Create().WithLine1("1 Some Ordering Party").Build();
            _organisationContact = null;
            _created = DateTime.UtcNow;
            _lastUpdated = DateTime.UtcNow;
            _lastUpdatedBy = Guid.NewGuid();
            _lastUpdatedByName = "Bob Smith";
            _supplierId = "Some supplier id";
            _supplierName = "Some supplier name";
            _supplierAddress = AddressBuilder.Create().WithLine1("1 Some Supplier").Build();
            _supplierContact = null;
            _commencementDate = null;
            _serviceRecipientsViewed = false;
            _catalogueSolutionsViewed = false;
            _additionalServicesViewed = false;
        }

        public static OrderBuilder Create() => new OrderBuilder();

        public OrderBuilder WithOrderId(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        public OrderBuilder WithDescription(string description)
        {
            _orderDescription = description;
            return this;
        }

        public OrderBuilder WithOrganisationId(Guid organisationId)
        {
            _organisationId = organisationId;
            return this;
        }

        public OrderBuilder WithOrganisationContact(Contact organisationContact)
        {
            _organisationContact = organisationContact;
            return this;
        }

        public OrderBuilder WithSupplierId(string supplierId)
        {
            _supplierId = supplierId;
            return this;
        }

        public OrderBuilder WithSupplierName(string supplierName)
        {
            _supplierName = supplierName;
            return this;
        }

        public OrderBuilder WithSupplierAddress(Address supplierAddress)
        {
            _supplierAddress = supplierAddress;
            return this;
        }

        public OrderBuilder WithSupplierContact(Contact supplierContact)
        {
            _supplierContact = supplierContact;
            return this;
        }

        public OrderBuilder WithCommencementDate(DateTime? commencementDate)
        {
            _commencementDate = commencementDate;
            return this;
        }

        public OrderBuilder WithServiceRecipientsViewed(bool serviceRecipientsViewed)
        {
            _serviceRecipientsViewed = serviceRecipientsViewed;
            return this;
        }

        public OrderBuilder WithCatalogueSolutionsViewed(bool catalogueSolutionsViewed)
        {
            _catalogueSolutionsViewed = catalogueSolutionsViewed;
            return this;
        }

        public OrderBuilder WithAdditionalServicesViewed(bool additionalServicesViewed)
        {
            _additionalServicesViewed = additionalServicesViewed;
            return this;
        }

        public OrderBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _lastUpdatedBy = lastUpdatedBy;
            return this;
        }

        public OrderBuilder WithLastUpdatedByName(string lastUpdatedByName)
        {
            _lastUpdatedByName = lastUpdatedByName;
            return this;
        }

        public OrderBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _lastUpdated = lastUpdated;
            return this;
        }

        public OrderBuilder WithOrderItem(OrderItem orderItem)
        {
            _orderItems.Add(orderItem);
            return this;
        }

        public OrderBuilder WithServiceRecipient((string Ods, string Name) serviceRecipient)
        {
            _serviceRecipients.Add(serviceRecipient);
            return this;
        }

        public Order Build()
        {
            var order = Order.Create(OrderDescription.Create(_orderDescription).Value, _organisationId, _lastUpdatedBy, _lastUpdatedByName);

            order.OrderId = _orderId;
            order.OrganisationName = _organisationName;
            order.OrganisationOdsCode = _organisationOdsCode;
            order.OrganisationAddress = _organisationAddress;
            order.OrganisationContact = _organisationContact;
            order.SupplierId = _supplierId;
            order.SupplierName = _supplierName;
            order.SupplierAddress = _supplierAddress;
            order.SupplierContact = _supplierContact;
            order.CommencementDate = _commencementDate;

            foreach (var orderItem in _orderItems)
            {
                order.AddOrderItem(orderItem, _lastUpdatedBy, _lastUpdatedByName);
            }

            order.SetServiceRecipient(
                _serviceRecipients,
                _lastUpdatedBy,
                _lastUpdatedByName);

            order.AdditionalServicesViewed = _additionalServicesViewed;
            order.ServiceRecipientsViewed = _serviceRecipientsViewed;
            order.CatalogueSolutionsViewed = _catalogueSolutionsViewed;
            order.Created = _created;
            order.LastUpdated = _lastUpdated;

            return order;
        }
    }
}
