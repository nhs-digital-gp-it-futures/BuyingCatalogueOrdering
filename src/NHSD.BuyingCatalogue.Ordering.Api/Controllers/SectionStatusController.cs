using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class SectionStatusController : ControllerBase
    {
        private static readonly Dictionary<string, Action<Order>> _completeSectionActionsDictionary = new()
        {
            { SectionModel.CatalogueSolutions.Id, o => o.CatalogueSolutionsViewed = true },
            { SectionModel.AdditionalServices.Id, o => o.AdditionalServicesViewed = true },
            { SectionModel.AssociatedServices.Id, o => o.AssociatedServicesViewed = true },
        };

        private readonly IOrderRepository _orderRepository;

        public SectionStatusController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpPut]
        [Route("{orderId}/sections/{sectionId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateStatusAsync(string orderId, string sectionId, UpdateOrderSectionModel sectionStatus)
        {
            if (sectionStatus is null)
            {
                throw new ArgumentNullException(nameof(sectionStatus));
            }

            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order is null)
            {
                return NotFound();
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            if (_completeSectionActionsDictionary.ContainsKey(sectionId))
            {
                if (sectionStatus.Status == "complete")
                {
                    _completeSectionActionsDictionary[sectionId](order);
                }
            }
            else
            {
                return Forbid();
            }

            order.SetLastUpdatedBy(User.GetUserId(), User.GetUserName());

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
