using System;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;
        private readonly ICreateOrderItemValidator _orderItemValidator;

        public CreateOrderItemService(IOrderRepository orderRepository,
            IIdentityService identityService,
            ICreateOrderItemValidator orderItemValidator)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
        }

        public async Task<Result<int>> CreateAsync(CreateOrderItemRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var validationErrors = _orderItemValidator.Validate(request).ToList();
            if (validationErrors.Any())
            {
                return Result.Failure<int>(validationErrors);
            }

            var provisioningType = ProvisioningType.FromName(request.ProvisioningTypeName);
            var cataloguePriceType = CataloguePriceType.FromName(request.CataloguePriceTypeName);
            var cataloguePriceUnit = CataloguePriceUnit.Create(request.CataloguePriceUnitTierName, request.CataloguePriceUnitDescription);

            var priceTimeUnit = TimeUnit.FromName(request.PriceTimeUnitName);
            var estimationPeriod = TimeUnit.FromName(request.EstimationPeriodName);

            var orderItem = new OrderItem(
                request.OdsCode, 
                request.CatalogueItemId,
                request.CatalogueItemType,
                request.CatalogueItemName,
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
                _identityService.GetUserIdentity(),
                _identityService.GetUserName());

            await _orderRepository.UpdateOrderAsync(order);

            return Result.Success(orderItem.OrderItemId);
        }
    }
}
