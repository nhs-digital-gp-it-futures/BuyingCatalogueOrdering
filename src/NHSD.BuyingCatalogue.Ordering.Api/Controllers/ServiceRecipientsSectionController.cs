﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

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
            _orderRepository = orderRepository;
            _serviceRecipientRepository = serviceRecipientRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceRecipientsModel>> GetAll(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order is null)
            {
                return NotFound();
            }

            var serviceRecipients = (await _serviceRecipientRepository.ListServiceRecipientsByOrderId(orderId)).ToList();

            var primaryOrganisationId = User.GetPrimaryOrganisationId();

            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            var recipientModelList = serviceRecipients.Select(recipient => new ServiceRecipientModel
            {
                OdsCode = recipient.OdsCode,
                Name = recipient.Name
            }).ToList();

            var model = new ServiceRecipientsModel
            {
                ServiceRecipients = recipientModelList
            };

            return model;
        }

        [HttpPut]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public ActionResult Update(string orderId, ServiceRecipientsModel model)
        {
            _cannedData[orderId] = model ?? throw new ArgumentNullException(nameof(model));

            return NoContent();
        }

        private static readonly Dictionary<string, ServiceRecipientsModel> _cannedData = new Dictionary<string, ServiceRecipientsModel>();
    }
}