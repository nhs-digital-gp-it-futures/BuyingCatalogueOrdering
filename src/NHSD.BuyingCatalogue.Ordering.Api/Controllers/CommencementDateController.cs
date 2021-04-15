using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{callOffId}/sections/commencement-date")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class CommencementDateController : ControllerBase
    {
        private readonly ICommencementDateService commencementDateService;

        public CommencementDateController(ICommencementDateService commencementDateService)
        {
            this.commencementDateService = commencementDateService ?? throw new ArgumentNullException(nameof(this.commencementDateService));
        }

        [HttpGet]
        public async Task<ActionResult<CommencementDateModel>> GetAsync(CallOffId callOffId)
        {
            var commencementDate = await commencementDateService.GetCommencementDate(callOffId);

            return new CommencementDateModel { CommencementDate = commencementDate };
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Order order, CommencementDateModel model)
        {
            if (order is null)
                return NotFound();

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            await commencementDateService.SetCommencementDate(order, model.CommencementDate);

            return NoContent();
        }
    }
}
