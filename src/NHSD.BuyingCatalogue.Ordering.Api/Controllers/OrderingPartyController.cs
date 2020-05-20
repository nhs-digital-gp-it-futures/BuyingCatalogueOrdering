using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/ordering-party")]
    [ApiController]
    [Produces("application/json")]
    public sealed class OrderingPartyController : Controller
    {
        [HttpGet]
        public ActionResult Get(string orderId)
        {
            var result = new OrderingPartyModel
            {
                Organisation = new OrganisationModel
                {
                    Name = $"Hampshire CC with orderId {orderId}",
                    OdsCode = "432432", 
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
                    }
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
    }
}
