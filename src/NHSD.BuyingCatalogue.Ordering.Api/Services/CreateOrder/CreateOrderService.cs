using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using Microsoft.AspNetCore.Http;


namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    sealed public class CreateOrderService : ICreateOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Result<string>> CreateAsync(CreateOrderRequest createOrderRequest)
        {
            if (createOrderRequest is null)
            {
                throw new ArgumentNullException(nameof(createOrderRequest));
            }

            var isDescriptionValid = OrderDescription.Create(createOrderRequest.Description);
            var isOrganisationValid = OrderOrganisationId.Create(createOrderRequest.OrganisationId);

            if (!isDescriptionValid.IsSuccess || !isOrganisationValid.IsSuccess)
            {
                var allErrors = isDescriptionValid.Errors.Union(isOrganisationValid.Errors);
                return Result.Failure<string>(allErrors);
            }

            var order = new Order {
                OrderStatus = new OrderStatus() { OrderStatusId = 2, Name = "Unsubmitted" },
                OrganisationId = isOrganisationValid.Value,
                LastUpdatedByName = createOrderRequest.LastUpdatedByName,
                LastUpdatedBy = createOrderRequest.LastUpdatedById,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            order.SetDescription(isDescriptionValid.Value);

            var orderId = await _orderRepository.CreateOrderAsync(order);
            return Result.Success(orderId);
        }
    }
}
