using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Results;
using NHSD.BuyingCatalogue.Ordering.Domain;
using static System.Int32;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    public class CreateOrderService : ICreateOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public CreateOrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<string>> CreateAsync(CreateOrderRequest createOrderRequest)
        {
            if (createOrderRequest is null)
            {
                throw new ArgumentNullException(nameof(createOrderRequest));
            }

            
            //TODO add a create method to Order to fill in defaults 

            var orderId = await GetNewOrderId();
            var newOrder = new Order {OrderId = orderId, OrganisationId = createOrderRequest.OrganisationId, Description = createOrderRequest.Description , OrderStatus = new OrderStatus() { OrderStatusId = 1, Name = "Submitted" } };

            //TODO validate order 

            await _orderRepository.CreateOrderAsync(newOrder);

            return Result.Success(newOrder.OrderId);
        }

        private async Task<string> GetNewOrderId()
        {
            var resultOrderId = "C0000014-01";
            var lastOrderId = await _orderRepository.GetLatestOrderByCreationDate();
            if (!string.IsNullOrEmpty(lastOrderId))
            {
                var numberSection = resultOrderId.Substring(1, 7);
                var orderNumber = Parse(s: numberSection, CultureInfo.InvariantCulture);
                resultOrderId = $"C{orderNumber+1:D6}-01";
            }
            return resultOrderId;
        }
    }
}
