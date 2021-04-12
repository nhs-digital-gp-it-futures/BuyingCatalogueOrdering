using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderModel
    {
        private OrderModel(Order order) => OrderItems = GetOrderItems(order);

        public string Description { get; init; }

        public OrderingPartyModel OrderParty { get; init; }

        public SupplierModel Supplier { get; init; }

        public DateTime? CommencementDate { get; init; }

        public IReadOnlyList<OrderItemModel> OrderItems { get; }

        public decimal TotalOneOffCost { get; init; }

        public decimal TotalRecurringCostPerMonth { get; init; }

        public decimal TotalRecurringCostPerYear { get; init; }

        public decimal TotalOwnershipCost { get; init; }

        public string Status { get; init; }

        public DateTime? DateCompleted { get; init; }

        internal static OrderModel Create(Order order)
        {
            var calculatedCostPerYear = order.CalculateCostPerYear(CostType.Recurring);

            return new OrderModel(order)
            {
                Description = order.Description,
                OrderParty = new OrderingPartyModel(order.OrderingParty, order.OrderingPartyContact),
                CommencementDate = order.CommencementDate,
                Supplier = new SupplierModel(order.Supplier, order.SupplierContact),
                TotalOneOffCost = order.CalculateCostPerYear(CostType.OneOff),
                TotalRecurringCostPerMonth = calculatedCostPerYear / 12,
                TotalRecurringCostPerYear = calculatedCostPerYear,
                TotalOwnershipCost = order.CalculateTotalOwnershipCost(),
                Status = order.OrderStatus.Name,
                DateCompleted = order.Completed,
            };
        }

        private static IReadOnlyList<OrderItemModel> GetOrderItems(Order order)
        {
            var items = new List<OrderItemModel>();
            var i = 1;

            foreach (var orderItem in order.OrderItems)
            {
                var recipientModels = new List<ExtendedOrderItemRecipientModel>();

                foreach (var recipient in orderItem.OrderItemRecipients)
                {
                    var odsCode = recipient.Recipient.OdsCode;
                    bool ServiceInstancePredicate(ServiceInstanceItem serviceInstanceItem) =>
                        serviceInstanceItem.CatalogueItemId == orderItem.CatalogueItem.Id.ToString()
                        && serviceInstanceItem.OdsCode == recipient.Recipient.OdsCode;

                    var recipientModel = new ExtendedOrderItemRecipientModel
                    {
                        DeliveryDate = recipient.DeliveryDate,
                        ItemId = $"{order.CallOffId}-{odsCode}-{i}",
                        Name = recipient.Recipient.Name,
                        OdsCode = odsCode,
                        Quantity = recipient.Quantity,
                        CostPerYear = recipient.CalculateTotalCostPerYear(orderItem.Price ?? 0, orderItem.EstimationPeriod),

                        // TODO: consider refactor
                        ServiceInstanceId = order.ServiceInstanceItems.FirstOrDefault(ServiceInstancePredicate)?.ServiceInstanceId,
                    };

                    recipientModels.Add(recipientModel);
                    i++;
                }

                items.Add(new OrderItemModel(orderItem, recipientModels));
            }

            return items;
        }
    }
}
