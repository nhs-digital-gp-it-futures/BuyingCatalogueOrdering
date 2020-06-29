using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem
{
    public sealed class UpdateOrderItemService : IUpdateOrderItemService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;

        public UpdateOrderItemService(
            IOrderRepository orderRepository,
            IIdentityService identityService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> UpdateAsync(UpdateOrderItemRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var estimationPeriod = TimeUnit.FromName(request.EstimationPeriodName);

            Order order = request.Order;
            order.UpdateOrderItem(
                request.OrderItemId, 
                request.DeliveryDate, 
                request.Quantity, 
                estimationPeriod,
                request.Price,
                _identityService.GetUserIdentity(),
                _identityService.GetUserName());

            await _orderRepository.UpdateOrderAsync(order);

            return Result.Success();
        }
    }
}
