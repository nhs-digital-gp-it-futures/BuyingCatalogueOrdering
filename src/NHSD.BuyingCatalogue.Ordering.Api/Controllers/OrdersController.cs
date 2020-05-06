using System;
using Microsoft.AspNetCore.Authorization;
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
            var orders = new OrdersModel()
            {
                OrderId = new Guid(),
                OrderDescription = "Some Order",
                LastUpdatedBy = "Bob Smith",
                LastUpdated = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow,
                Status = "Unsubmitted"
            };

            return Ok(orders);
        }
    }
}
