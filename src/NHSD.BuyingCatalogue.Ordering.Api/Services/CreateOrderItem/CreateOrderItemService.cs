using System;
using System.Collections.Generic;
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
        private readonly IServiceRecipientRepository serviceRecipientRepository;

        public CreateOrderItemService(
            IOrderRepository orderRepository,
            IIdentityService identityService,
            IOrderItemFactory orderItemFactory,
            ICreateOrderItemValidator orderItemValidator,
            IServiceRecipientRepository serviceRecipientRepository)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.orderItemFactory = orderItemFactory ?? throw new ArgumentNullException(nameof(orderItemFactory));
            this.orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
            this.serviceRecipientRepository = serviceRecipientRepository ?? throw new ArgumentNullException(nameof(serviceRecipientRepository));
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
            order.AddOrderItem(orderItem,
                identityService.GetUserIdentity(),
                identityService.GetUserName());

            await orderRepository.UpdateOrderAsync(order);

            return Result.Success(orderItem.OrderItemId);
        }

        public async Task<AggregateValidationResult> CreateAsync(Order order, IEnumerable<CreateOrderItemRequest> requests)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (requests is null)
                throw new ArgumentNullException(nameof(requests));

            var userId = identityService.GetUserIdentity();
            var userName = identityService.GetUserName();

            var serviceRecipients = new HashSet<ServiceRecipient>();
            var aggregateValidationResult = new AggregateValidationResult();

            var itemIndex = -1;

            foreach (var request in requests)
            {
                itemIndex++;

                var validationResult = orderItemValidator.Validate(request);
                aggregateValidationResult.AddValidationResult(validationResult, itemIndex);

                if (!validationResult.Success)
                    continue;

                if (request.ServiceRecipient != null)
                    serviceRecipients.Add(request.ServiceRecipient);

                var orderItem = orderItemFactory.Create(request);
                order.AddOrderItem(orderItem, userId, userName);
            }

            if (!aggregateValidationResult.Success)
                return aggregateValidationResult;

            await serviceRecipientRepository.UpdateWithoutSavingAsync(order.OrderId, serviceRecipients);
            await orderRepository.UpdateOrderAsync(order);

            return aggregateValidationResult;
        }
    }
}
