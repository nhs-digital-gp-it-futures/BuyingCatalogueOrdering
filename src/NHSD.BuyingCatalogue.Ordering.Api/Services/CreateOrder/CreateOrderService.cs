using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    public sealed class CreateOrderService : ICreateOrderService
    {
        private readonly IOrderRepository orderRepository;

        public CreateOrderService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
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

            var order = Order.Create(
                isDescriptionValid.Value,
                isOrganisationValid.Value,
                createOrderRequest.LastUpdatedById,
                createOrderRequest.LastUpdatedByName);

            var orderId = await orderRepository.CreateOrderAsync(order);
            return Result.Success(orderId);
        }
    }
}
