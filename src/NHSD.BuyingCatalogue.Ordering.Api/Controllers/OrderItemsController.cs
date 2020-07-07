using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class OrderItemsController : ControllerBase
    {
        [HttpGet]
        [Route("order-items")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Swagger does not allow static files")]
        public ActionResult<List<GetOrderItemModel>> Get(string orderId, [FromQuery] string catalogueItemType)
        {
            if (orderId is null)
                return NotFound();

            if (catalogueItemType is null)
                throw new ArgumentNullException(nameof(catalogueItemType));

            return new List<GetOrderItemModel>();
        }
    }
}
