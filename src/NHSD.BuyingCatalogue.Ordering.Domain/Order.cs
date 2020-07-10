using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Order
    {
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();
        private readonly List<ServiceRecipient> _serviceRecipients = new List<ServiceRecipient>();

        private Order()
        {
        }

        private Order(OrderDescription orderDescription, Guid organisationId) : this()
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
            OrganisationId = organisationId;
            OrderStatus = OrderStatus.Unsubmitted;
            Created = DateTime.UtcNow;
        }

        public static Order Create(
            OrderDescription orderDescription,
            Guid organisationId,
            Guid lastUpdatedBy,
            string lastUpdatedByName)
        {
            var order = new Order(orderDescription, organisationId);
            order.SetLastUpdatedBy(lastUpdatedBy, lastUpdatedByName);
            return order;
        }

        public string OrderId { get; set; }

        public OrderDescription Description { get; private set; }

        public Guid OrganisationId { get; }

        public string OrganisationName { get; set; }

        public string OrganisationOdsCode { get; set; }

        [ForeignKey("OrganisationAddressId")]
        public Address OrganisationAddress { get; set; }

        [ForeignKey("OrganisationContactId")]
        public Contact OrganisationContact { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public string LastUpdatedByName { get; set; }

        public OrderStatus OrderStatus { get; }

        public bool ServiceRecipientsViewed { get; set; }

        public bool CatalogueSolutionsViewed { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

        [ForeignKey("SupplierAddressId")]
        public Address SupplierAddress { get; set; }

        [ForeignKey("SupplierContactId")]
        public Contact SupplierContact { get; set; }

        public DateTime? CommencementDate { get; set; }

        public bool AdditionalServicesViewed { get; set; }

        public IReadOnlyList<OrderItem> OrderItems =>
            _orderItems.AsReadOnly();

        public IReadOnlyList<ServiceRecipient> ServiceRecipients =>
            _serviceRecipients.AsReadOnly();

        public void SetDescription(OrderDescription orderDescription)
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
        }

        public void SetLastUpdatedBy(Guid userId, string name)
        {
            LastUpdatedBy = userId;
            LastUpdatedByName = name ?? throw new ArgumentNullException(nameof(name));
            LastUpdated = DateTime.UtcNow;
        }

        public void AddOrderItem(
            OrderItem orderItem,
            Guid userId,
            string name)
        {
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem));

            if (_orderItems.Contains(orderItem))
                return;

            _orderItems.Add(orderItem);
            CatalogueSolutionsViewed = true;
            SetLastUpdatedBy(userId, name);
        }

        public void UpdateOrderItem(
            int orderItemId,
            DateTime? deliveryDate, 
            int quantity, 
            TimeUnit estimationPeriod, 
            decimal? price,
            Guid userId, 
            string name)
        {
            var orderItem = _orderItems.FirstOrDefault(item => orderItemId.Equals(item.OrderItemId));

            orderItem?.ChangePrice(
                deliveryDate, 
                quantity, 
                estimationPeriod, 
                price,
                () => SetLastUpdatedBy(userId, name));
        }

        public void SetServiceRecipient(IEnumerable<(string Ods, string Name)> serviceRecipients, Guid userId, string lastUpdatedName)
        {
            if (serviceRecipients is null)
                throw new ArgumentNullException(nameof(serviceRecipients));

            _serviceRecipients.Clear();

            foreach ((string ods, string name) in serviceRecipients)
            {
                _serviceRecipients.Add(new ServiceRecipient
                {
                    Name = name,
                    OdsCode = ods,
                    Order = this
                });
            }

            SetLastUpdatedBy(userId, lastUpdatedName);
        }
    }
}
