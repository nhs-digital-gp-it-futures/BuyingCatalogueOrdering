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
    [Route("api/v1/orders/{callOffId}/funding-source")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class FundingSourceController : Controller
    {
        private readonly IFundingSourceService fundingSourceService;

        public FundingSourceController(IFundingSourceService fundingSourceService)
        {
            this.fundingSourceService = fundingSourceService ?? throw new ArgumentNullException(nameof(fundingSourceService));
        }

        [HttpGet]
        public async Task<ActionResult<GetFundingSourceModel>> GetAsync(CallOffId callOffId)
        {
            bool? onlyGms = await fundingSourceService.GetFundingSource(callOffId);

            return new GetFundingSourceModel { OnlyGms = onlyGms };
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> PutFundingSourceAsync([FromRoute] Order order, UpdateFundingSourceModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (order is null)
                return NotFound();

            await fundingSourceService.SetFundingSource(order, model.OnlyGms);

            return NoContent();
        }
    }
}
