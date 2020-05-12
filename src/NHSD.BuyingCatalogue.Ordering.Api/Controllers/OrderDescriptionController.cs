using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/description")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class OrderDescriptionController : Controller
    {
        [HttpGet]
        public ActionResult Get([FromRoute][Required] string orderId)
        {
            var descriptionModel = new OrderDescriptionModel()
            {
                Description = _cannedData.Keys.Contains(orderId) ? _cannedData[orderId] : "This is some description"
            };

            return Ok(descriptionModel);
        }

        [HttpPut]
        public ActionResult Update([FromRoute][Required]string orderId, [FromBody][Required] OrderDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            _cannedData[orderId] = model.Description;
            return NoContent();
        }

        private static readonly Dictionary<string, string> _cannedData = new Dictionary<string, string>();
    }
}
