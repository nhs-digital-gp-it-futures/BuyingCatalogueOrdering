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
    [Authorize(Policy =  PolicyName.CanAccessOrders)]
    public sealed class OrderItemsController : ControllerBase
    {
        [HttpGet]
        [Route("order-items")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Swagger does not allow static files")]
        public ActionResult<List<string>> Get(string orderId)
        {
            if(orderId is null)
                throw new ArgumentNullException(nameof(orderId));

            return new List<string>();
        }

        [HttpPut]
        [Route("sections/{sectionId}")]
        public ActionResult Update(string orderId, string sectionId, UpdateOrderSection model)
        {
            if(model is null)
                throw new ArgumentNullException(nameof(model));

            if(orderId is null)
                throw new ArgumentNullException(nameof(orderId));

            if(sectionId is null)
                throw new ArgumentNullException(nameof(sectionId));

            return NoContent();
        }
    }
}
