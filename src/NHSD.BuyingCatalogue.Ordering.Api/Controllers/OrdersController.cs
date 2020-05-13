using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class OrdersController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetOrders()
        {
            var orders = new List<OrdersModel>
            {
                new OrdersModel
                {
                    OrderId = "C0000014-01",
                    OrderDescription = "Some Order",
                    LastUpdatedBy = "Bob Smith",
                    LastUpdated = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    Status = "Unsubmitted"
                },
                new OrdersModel
                {
                    OrderId = "C000012-01",
                    OrderDescription = "Some new order",
                    LastUpdatedBy = "Alice Smith",
                    LastUpdated = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    Status = "Submitted"
                }
            };

            return Ok(orders);
        }

        [HttpPost]
        public ActionResult<CreateOrderResponseModel> CreateOrderAsync([FromBody][Required] CreateOrderModel order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            var createOrderResponse = new CreateOrderResponseModel {OrderId = "C0000014-01" };
            return Ok(createOrderResponse);
        }
    }
}
