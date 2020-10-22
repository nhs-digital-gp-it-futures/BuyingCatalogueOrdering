using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem
{
    public sealed class UpdateOrderItemService : IUpdateOrderItemService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IIdentityService identityService;
        private readonly IUpdateOrderItemValidator orderItemValidator;

        public UpdateOrderItemService(
            IOrderRepository orderRepository,
            IIdentityService identityService,
            IUpdateOrderItemValidator orderItemValidator)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
        }

        public async Task<Result> UpdateAsync(
            UpdateOrderItemRequest request,
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var estimationPeriod = request.EstimationPeriod;

            Order order = request.Order;

            var validationErrors = orderItemValidator.Validate(request, catalogueItemType, provisioningType).ToList();
            if (validationErrors.Any())
            {
                return Result.Failure(validationErrors);
            }

            order.UpdateOrderItem(
                request.OrderItemId,
                request.DeliveryDate,
                request.Quantity,
                estimationPeriod,
                request.Price,
                identityService.GetUserIdentity(),
                identityService.GetUserName());

            await orderRepository.UpdateOrderAsync(order);

            return Result.Success();
        }
    }
}
