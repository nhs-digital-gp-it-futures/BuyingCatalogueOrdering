using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Order
    {
        private readonly List<OrderItem> orderItems = new List<OrderItem>();
        private readonly List<ServiceRecipient> serviceRecipients = new List<ServiceRecipient>();

#pragma warning disable 649
        private DateTime? _completed;
#pragma warning restore 649

        private Order()
        {
        }

        private Order(OrderDescription orderDescription, Guid organisationId) : this()
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
            OrganisationId = organisationId;
            OrderStatus = OrderStatus.Incomplete;
            Created = DateTime.UtcNow;
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

        /// <summary>
        /// Gets the completed date and time.
        /// </summary>
        /// <remarks>
        /// Do not need to convert this to an auto property as recommended by ReSharper.
        /// ReSharper disable once ConvertToAutoProperty
        /// </remarks>
        public DateTime? Completed
        {
            get
            {
                return _completed;
            }
            private set
            {
                _completed = value;
            }
        }

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

        public bool AdditionalServicesViewed { get; set; }

        public bool AssociatedServicesViewed { get; set; }

        public bool? FundingSourceOnlyGMS { get; set; }

        public bool IsDeleted { get; set; }

        public IReadOnlyList<OrderItem> OrderItems =>
            orderItems.AsReadOnly();

        public IReadOnlyList<ServiceRecipient> ServiceRecipients =>
            serviceRecipients.AsReadOnly();

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

        public void SetDescription(OrderDescription orderDescription)
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
        }

        public decimal CalculateCostPerYear(CostType costType)
        {
            return orderItems.Where(x => x.CostType == costType).Sum(y => y.CalculateTotalCostPerYear());
        }

        public decimal CalculateTotalOwnershipCost()
        {
            const int defaultContractLength = 3;

            return CalculateCostPerYear(CostType.OneOff) + (defaultContractLength * CalculateCostPerYear(CostType.Recurring));
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

            if (orderItems.Contains(orderItem))
                return;

            orderItems.Add(orderItem);
            orderItem.MarkOrderSectionAsViewed(this);

            SetLastUpdatedBy(userId, name);
        }

        public void UpdateOrderItem(
            int orderItemId,
            DateTime? deliveryDate,
            int quantity,
            TimeUnit? estimationPeriod,
            decimal? price,
            Guid userId,
            string name)
        {
            var orderItem = orderItems.FirstOrDefault(item => orderItemId.Equals(item.OrderItemId));

            orderItem?.ChangePrice(
                deliveryDate,
                quantity,
                estimationPeriod,
                price,
                () => SetLastUpdatedBy(userId, name));
        }

        public void SetServiceRecipients(IEnumerable<OdsOrganisation> recipients, Guid userId, string lastUpdatedName)
        {
            if (recipients is null)
                throw new ArgumentNullException(nameof(recipients));

            serviceRecipients.Clear();

            foreach ((string code, string name) in recipients)
            {
                serviceRecipients.Add(new ServiceRecipient
                {
                    Name = name,
                    OdsCode = code,
                    Order = this
                });
            }

            SetLastUpdatedBy(userId, lastUpdatedName);
        }

        public bool CanComplete()
        {
            if (!FundingSourceOnlyGMS.HasValue)
                return false;

            int serviceRecipientsCount = ServiceRecipients.Count;
            int catalogueSolutionsCount = OrderItems.Count(o => o.CatalogueItemType.Equals(CatalogueItemType.Solution));
            int associatedServicesCount = OrderItems.Count(o => o.CatalogueItemType.Equals(CatalogueItemType.AssociatedService));

            var solutionAndAssociatedServices = catalogueSolutionsCount > 0
                                                && associatedServicesCount > 0;

            var solutionAndNoAssociatedServices = catalogueSolutionsCount > 0
                                                  && associatedServicesCount == 0
                                                  && AssociatedServicesViewed;

            var noSolutionsAndAssociatedServices = serviceRecipientsCount > 0
                                                   && catalogueSolutionsCount == 0
                                                   && CatalogueSolutionsViewed
                                                   && associatedServicesCount > 0;

            var recipientsAndAssociatedServices = serviceRecipientsCount == 0
                                                   && ServiceRecipientsViewed
                                                   && associatedServicesCount > 0;

            return solutionAndNoAssociatedServices || solutionAndAssociatedServices || recipientsAndAssociatedServices || noSolutionsAndAssociatedServices;
        }

        public Result Complete(Guid lastUpdatedBy, string lastUpdatedByName)
        {
            if (!CanComplete())
                return Result.Failure(OrderErrors.OrderNotComplete());

            OrderStatus = OrderStatus.Complete;
            Completed = DateTime.UtcNow;

            SetLastUpdatedBy(lastUpdatedBy, lastUpdatedByName);

            return Result.Success();
        }
    }
}
