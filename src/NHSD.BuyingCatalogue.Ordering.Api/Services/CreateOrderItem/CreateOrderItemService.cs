using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public sealed class CreateOrderItemService : ICreateOrderItemService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IIdentityService identityService;
        private readonly IOrderItemFactory orderItemFactory;
        private readonly ICreateOrderItemValidator orderItemValidator;

        public CreateOrderItemService(
            IOrderRepository orderRepository,
            IIdentityService identityService,
            IOrderItemFactory orderItemFactory,
            ICreateOrderItemValidator orderItemValidator)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.orderItemFactory = orderItemFactory ?? throw new ArgumentNullException(nameof(orderItemFactory));
            this.orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
        }

        public async Task<Result<int>> CreateAsync(CreateOrderItemRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var validationResult = orderItemValidator.Validate(request);
            if (!validationResult.Success)
            {
                return Result.Failure<int>(validationResult.Errors);
            }

            var catalogueItemType = request.CatalogueItemType;
            var provisioningType = request.ProvisioningType;
            var cataloguePriceUnit = CataloguePriceUnit.Create(request.CataloguePriceUnitTierName, request.CataloguePriceUnitDescription);

            var priceTimeUnit = request.PriceTimeUnit;
            var estimationPeriod = catalogueItemType.InferEstimationPeriod(provisioningType, request.EstimationPeriod);

            var orderItem = new OrderItem(
                request.OdsCode,
                request.CatalogueItemId,
                catalogueItemType,
                request.CatalogueItemName,
                request.CatalogueSolutionId,
                provisioningType,
                request.CataloguePriceType,
                cataloguePriceUnit,
                priceTimeUnit,
                request.CurrencyCode,
                request.Quantity,
                estimationPeriod,
                request.DeliveryDate,
                request.Price);

            Order order = request.Order;
            order.AddOrderItem(
                orderItem,
                identityService.GetUserIdentity(),
                identityService.GetUserName());

            await orderRepository.UpdateOrderAsync(order);

            return Result.Success(orderItem.OrderItemId);
        }

        public async Task<AggregateValidationResult> CreateAsync(Order order, IReadOnlyList<CreateOrderItemRequest> model)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var aggregateValidationResult = orderItemValidator.Validate(model, order.OrderItems);
            if (!aggregateValidationResult.Success)
                return aggregateValidationResult;

            order.MergeOrderItems(CreateOrderItemMerge(model));
            await orderRepository.UpdateOrderAsync(order);

            return aggregateValidationResult;
        }

        private OrderItemMerge CreateOrderItemMerge(IReadOnlyList<CreateOrderItemRequest> requests)
        {
            var serviceRecipients = requests.Select(r => r.ServiceRecipient).Where(r => r is not null);
            (Guid id, string name) = identityService.GetUserInfo();

            var orderItemsToMerge = new OrderItemMerge(serviceRecipients, id, name);

            if (!orderItemsToMerge.AddOrderItems(requests.Select(r => orderItemFactory.Create(r))))
            {
                // This should never happen under normal circumstances
                // as validation should prevent it
                throw new InvalidOperationException("Duplicate order item IDs exist.");
            }

            return orderItemsToMerge;
        }
    }
}
