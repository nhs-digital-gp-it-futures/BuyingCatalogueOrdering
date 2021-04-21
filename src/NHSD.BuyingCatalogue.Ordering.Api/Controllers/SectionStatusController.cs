using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Authorization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    [AuthorizeOrganisation]
    public sealed class SectionStatusController : ControllerBase
    {
        private static readonly HashSet<string> SectionsSet = new()
        {
            SectionModel.CatalogueSolutions.Id,
            SectionModel.AdditionalServices.Id,
            SectionModel.AssociatedServices.Id,
        };

        private readonly ISectionStatusService sectionStatusService;

        public SectionStatusController(ISectionStatusService sectionStatusService)
        {
            this.sectionStatusService = sectionStatusService ?? throw new ArgumentNullException(nameof(sectionStatusService));
        }

        [HttpPut]
        [Route("{callOffId}/sections/{sectionId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<IActionResult> UpdateStatusAsync(
            CallOffId callOffId,
            string sectionId,
            UpdateOrderSectionModel sectionStatus)
        {
            if (sectionStatus is null)
                throw new ArgumentNullException(nameof(sectionStatus));

            if (sectionId is null)
                throw new ArgumentNullException(nameof(sectionId));

            var order = await sectionStatusService.GetOrder(callOffId);

            if (order is null)
                return NotFound();

            if (SectionsSet.Contains(sectionId))
            {
                if (sectionStatus.Status == "complete")
                    await sectionStatusService.SetSectionStatus(order, sectionId);
            }
            else
            {
                return Forbid();
            }

            return NoContent();
        }
    }
}
