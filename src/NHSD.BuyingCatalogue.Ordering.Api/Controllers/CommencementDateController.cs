using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/commencement-date")]
    [ApiController]
    [Produces("application/json")]
    public sealed class CommencementDateController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            var result = new CommencementDateModel { CommencementDate = DateTime.UtcNow };

            return Ok(result);
        }

        [HttpPut]
        public ActionResult Update(string orderId, CommencementDateModel model)
        {
            if (orderId is null)
            {
                return NotFound();
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return NoContent();
        }
    }
}
