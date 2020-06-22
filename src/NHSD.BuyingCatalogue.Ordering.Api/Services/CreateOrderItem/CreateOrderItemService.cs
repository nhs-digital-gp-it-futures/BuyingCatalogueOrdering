using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    public sealed class CreateOrderItemService : ICreateOrderItemService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;

        public CreateOrderItemService(IOrderRepository orderRepository,
            IIdentityService identityService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task CreateAsync(CreateOrderItemRequest createOrderItemRequest)
        {
            if (createOrderItemRequest is null)
                throw new ArgumentNullException(nameof(createOrderItemRequest));

            Order order = createOrderItemRequest.Order;
            var orderItem = new OrderItem(
                createOrderItemRequest.OdsCode, 
                createOrderItemRequest.CatalogueItemId,
                createOrderItemRequest.CatalogueItemType,
                createOrderItemRequest.CatalogueItemName,
                createOrderItemRequest.ProvisioningType,
                createOrderItemRequest.CataloguePriceUnit,
                createOrderItemRequest.PriceUnit,
                createOrderItemRequest.CurrencyCode,
                createOrderItemRequest.Quantity,
                createOrderItemRequest.EstimationPeriod,
                createOrderItemRequest.DeliveryDate,
                createOrderItemRequest.Price);

            order.AddOrderItem(orderItem, 
                _identityService.GetUserIdentity(), 
                _identityService.GetUserName());

            await _orderRepository.UpdateOrderAsync(order);
        }
    }
}
