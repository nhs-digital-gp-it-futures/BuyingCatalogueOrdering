using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var validationErrors = orderItemValidator.Validate(request).ToList();
            if (validationErrors.Any())
            {
                return Result.Failure<int>(validationErrors);
            }

            var catalogueItemType = request.CatalogueItemType;
            var provisioningType = request.ProvisioningType.Value;
            var cataloguePriceType = request.CataloguePriceType.Value;
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
                cataloguePriceType,
                cataloguePriceUnit,
                priceTimeUnit,
                request.CurrencyCode,
                request.Quantity,
                estimationPeriod,
                request.DeliveryDate,
                request.Price);

            Order order = request.Order;
            order.AddOrderItem(orderItem,
                identityService.GetUserIdentity(),
                identityService.GetUserName());

            await orderRepository.UpdateOrderAsync(order);

            return Result.Success(orderItem.OrderItemId);
        }

        public async Task CreateAsync(Order order, IEnumerable<CreateOrderItemRequest> requests)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (requests is null)
                throw new ArgumentNullException(nameof(requests));

            var userId = identityService.GetUserIdentity();
            var userName = identityService.GetUserName();

            foreach (var request in requests)
            {
                var orderItem = orderItemFactory.Create(request);
                order.AddOrderItem(orderItem, userId, userName);
            }

            await orderRepository.UpdateOrderAsync(order);
        }
    }
}
