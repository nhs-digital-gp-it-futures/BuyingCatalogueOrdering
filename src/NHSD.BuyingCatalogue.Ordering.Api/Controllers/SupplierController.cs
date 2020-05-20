using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/supplier")]
    [ApiController]
    [Produces("application/json")]
    public sealed class SupplierController : Controller
    {
        [HttpGet]
        public ActionResult Get(string orderId)
        {
            var result = new SupplierModel
            {
                SupplierId = $"SupplierId {orderId}",
                Name = "Some name",
                Address = new AddressModel
                {
                    Line1 = "Some address",
                    Line2 = "Some Road",
                    Line3 = "Line 3 address",
                    Line4 = "Another line of address",
                    Line5 = "5th line of address",
                    Town = "A Town",
                    County = "Some county",
                    Postcode = "Some postcode",
                    Country = "A country"
                },
                PrimaryContact = new PrimaryContactModel
                {
                    FirstName = "bob",
                    LastName = "smith",
                    EmailAddress = "bob.smith@email.com",
                    TelephoneNumber = "4342345223  3434324"
                }
            };

            return Ok(result);
        }

        [HttpPut]
        public ActionResult Update([FromRoute] [Required] string orderId, [FromBody] [Required] SupplierModel model)
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
