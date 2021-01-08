﻿using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/orders/{orderId}/sections/service-recipients")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class ServiceRecipientsSectionController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IServiceRecipientRepository _serviceRecipientRepository;

        public ServiceRecipientsSectionController(IOrderRepository orderRepository,
            IServiceRecipientRepository serviceRecipientRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _serviceRecipientRepository = serviceRecipientRepository ??
                                          throw new ArgumentNullException(nameof(serviceRecipientRepository));
        }

        [HttpGet]
        public async Task<ActionResult<ServiceRecipientsModel>> GetAllAsync(string orderId)
        {
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

            var serviceRecipients =
                (await _serviceRecipientRepository.ListServiceRecipientsByOrderIdAsync(orderId)).ToList();

            var recipientModelList = serviceRecipients
                .Select(recipient => new ServiceRecipientModel
                {
                    OdsCode = recipient.OdsCode,
                    Name = recipient.Name,
                })
                .OrderBy(serviceRecipientModel => serviceRecipientModel.Name)
                .ToList();

            var model = new ServiceRecipientsModel
            {
                ServiceRecipients = recipientModelList,
            };

            return model;
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> UpdateAsync(string orderId, ServiceRecipientsModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
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

            var serviceRecipients = model.ServiceRecipients.Select(recipient => new ServiceRecipient
            {
                Name = recipient.Name,
                OdsCode = recipient.OdsCode,
                Order = order,
            }).ToList();

            await _serviceRecipientRepository.UpdateAsync(order.OrderId, serviceRecipients);

            if (serviceRecipients.Count is 0)
            {
                order.CatalogueSolutionsViewed = false;
            }

            order.ServiceRecipientsViewed = true;
            order.SetLastUpdatedBy(User.GetUserId(), User.GetUserName());

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
