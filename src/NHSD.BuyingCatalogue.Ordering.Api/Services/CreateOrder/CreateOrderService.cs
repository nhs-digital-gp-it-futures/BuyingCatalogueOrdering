using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    public class CreateOrderService : ICreateOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly  IHttpContextAccessor _httpContextAccessor;

        public CreateOrderService(IOrderRepository orderRepository , IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<string>> CreateAsync(CreateOrderRequest createOrderRequest)
        {
            if (createOrderRequest is null)
            {
                throw new ArgumentNullException(nameof(createOrderRequest));
            }

            //TODO Move this to a builder
            var order = new Order {
                OrderStatus = new OrderStatus() { OrderStatusId=2, Name = "Unsubmitted" },
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
            };

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                order.LastUpdatedBy = Guid.Parse(userId);
            }

            var isDescriptionValid = OrderDescription.Create(createOrderRequest.Description);
            if (isDescriptionValid.IsSuccess)
            {
                order.SetDescription(isDescriptionValid.Value);
            }
                       
            var isOrganisationValid = OrderOrganisationId.Create(createOrderRequest.OrganisationId);
            if (isOrganisationValid.IsSuccess)
            {
                order.OrganisationId = isOrganisationValid.Value;
            }

            if (!isDescriptionValid.IsSuccess || !isOrganisationValid.IsSuccess )
            {
                var allErrors = isDescriptionValid.Errors.Union(isOrganisationValid.Errors);
                return  Result.Failure<string>(allErrors);
            }

            var orderId = await _orderRepository.CreateOrderAsync(order);

            return Result.Success(orderId);
        }
    }
}
