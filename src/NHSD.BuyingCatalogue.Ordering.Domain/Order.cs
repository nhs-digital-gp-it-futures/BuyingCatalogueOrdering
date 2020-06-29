using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Order
    {
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public string OrderId { get; set; }

        public OrderDescription Description { get; private set; }

        public Guid OrganisationId { get; set; }

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

        public OrderStatus OrderStatus { get; set; }

        public bool ServiceRecipientsViewed { get; set; }

        public bool CatalogueSolutionsViewed { get; set; }

        public string SupplierId { get; set; } 

        public string SupplierName { get; set; }

        [ForeignKey("SupplierAddressId")]
        public Address SupplierAddress { get; set; }

        [ForeignKey("SupplierContactId")]
        public Contact SupplierContact { get; set; }

        public DateTime? CommencementDate { get; set; }

        public IReadOnlyList<OrderItem> OrderItems => 
            _orderItems.AsReadOnly();

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

            _orderItems.Add(orderItem);

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
    }
}
