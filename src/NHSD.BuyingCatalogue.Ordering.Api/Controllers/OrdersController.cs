﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICreateOrderService _createOrderService;
        private readonly IServiceRecipientRepository _serviceRecipientRepository;

        public OrdersController(
            IOrderRepository orderRepository,
            ICreateOrderService createOrderService,
            IServiceRecipientRepository serviceRecipientRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _createOrderService = createOrderService ?? throw new ArgumentNullException(nameof(createOrderService));
            _serviceRecipientRepository = serviceRecipientRepository ?? throw new ArgumentNullException(nameof(serviceRecipientRepository));
        }

        [HttpGet]
        [Route("{orderId}")]
        public async Task<ActionResult<OrderModel>> GetAsync(string orderId)
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

            var serviceRecipientDictionary = order.ServiceRecipients.Select(serviceRecipient =>
                    new ServiceRecipientModel { Name = serviceRecipient.Name, OdsCode = serviceRecipient.OdsCode })
                .ToDictionary(x => x.OdsCode);

            var orderItems = order.OrderItems;
            var orderOrganisationOdsCode = order.OrganisationOdsCode;

            if (orderItems.Select(x => x.OdsCode).Contains(orderOrganisationOdsCode))
            {
                if (!serviceRecipientDictionary.ContainsKey(orderOrganisationOdsCode))
                {
                    serviceRecipientDictionary.TryAdd(orderOrganisationOdsCode.ToUpperInvariant(),
                        new ServiceRecipientModel { Name = order.OrganisationName, OdsCode = orderOrganisationOdsCode });
                }
            }

            const int monthsPerYear = 12;
            var calculatedCostPerYear = order.CalculateCostPerYear(CostType.Recurring);
            var totalOneOffCost = order.CalculateCostPerYear(CostType.OneOff);

            return new OrderModel
            {
                Description = order.Description.Value,
                OrderParty = new OrderingPartyModel
                {
                    Name = order.OrganisationName,
                    OdsCode = order.OrganisationOdsCode,
                    Address = order.OrganisationAddress.ToModel(),
                    PrimaryContact = order.OrganisationContact.ToModel()
                },
                CommencementDate = order.CommencementDate,
                Supplier = new SupplierModel
                {
                    Name = order.SupplierName,
                    Address = order.SupplierAddress.ToModel(),
                    PrimaryContact = order.SupplierContact.ToModel()
                },
                TotalOneOffCost = totalOneOffCost,
                TotalRecurringCostPerMonth = calculatedCostPerYear / monthsPerYear,
                TotalRecurringCostPerYear = calculatedCostPerYear,
                TotalOwnershipCost = order.CalculateTotalOwnershipCost(),
                ServiceRecipients = serviceRecipientDictionary.Values,
                Status = order.OrderStatus.Name,
                DateCompleted = order.Completed,
                OrderItems = order.OrderItems.Select(orderItem =>
                    new OrderItemModel
                    {
                        ItemId = $"{order.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                        ServiceRecipientsOdsCode = orderItem.OdsCode,
                        CataloguePriceType = orderItem.CataloguePriceType.Name,
                        CatalogueItemType = orderItem.CatalogueItemType.Name,
                        CatalogueItemName = orderItem.CatalogueItemName,
                        ProvisioningType = orderItem.ProvisioningType.Name,
                        ItemUnitDescription = orderItem.CataloguePriceUnit.Description,
                        TimeUnitDescription = orderItem.PriceTimeUnit?.Description,
                        QuantityPeriodDescription = orderItem.EstimationPeriod?.Description,
                        DeliveryDate = orderItem.DeliveryDate,
                        Price = orderItem.Price,
                        Quantity = orderItem.Quantity,
                        CostPerYear = orderItem.CalculateTotalCostPerYear()
                    })
            };
        }

        [HttpGet]
        [Route("/api/v1/organisations/{organisationId}/[controller]")]
        public async Task<ActionResult<IList<OrderListItemModel>>> GetAllAsync(Guid organisationId)
        {
            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != organisationId)
            {
                return Forbid();
            }

            var orders = await _orderRepository.ListOrdersByOrganisationIdAsync(organisationId);
            var orderModelResult = orders.Select(order => new OrderListItemModel
            {
                OrderId = order.OrderId,
                Description = order.Description.Value,
                LastUpdatedBy = order.LastUpdatedByName,
                LastUpdated = order.LastUpdated,
                DateCreated = order.Created,
                DateCompleted = order.Completed,
                Status = order.OrderStatus.Name,
                OnlyGms = order.FundingSourceOnlyGMS
            }).ToList();

            return orderModelResult;
        }

        [HttpGet]
        [Route("{orderId}/summary")]
        public async Task<ActionResult<OrderSummaryModel>> GetOrderSummaryAsync(string orderId)
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

            int serviceRecipientsCount = await _serviceRecipientRepository.GetCountByOrderIdAsync(orderId);
            int catalogueSolutionsCount = order.OrderItems.Count(y => y.CatalogueItemType.Equals(CatalogueItemType.Solution));
            int associatedServicesCount = order.OrderItems.Count(y => y.CatalogueItemType.Equals(CatalogueItemType.AssociatedService));
            int additionalServicesCount = order.OrderItems.Count(y => y.CatalogueItemType.Equals(CatalogueItemType.AdditionalService));

            OrderSummaryModel orderSummaryModel = new OrderSummaryModel
            {
                OrderId = orderId,
                OrganisationId = order.OrganisationId,
                Description = order.Description.Value,
                Sections = new List<SectionModel>
                {
                    SectionModel.Description,
                    SectionModel.OrderingParty.WithStatus(order.IsOrderingPartySectionComplete() ? "complete" : "incomplete"),
                    SectionModel.Supplier.WithStatus(order.IsSupplierSectionComplete() ? "complete" : "incomplete"),
                    SectionModel.CommencementDate.WithStatus(order.IsCommencementDateSectionComplete() ? "complete" : "incomplete"),
                    SectionModel.AssociatedServices
                        .WithStatus(order.IsAssociatedServicesSectionComplete() ? "complete" : "incomplete")
                        .WithCount(associatedServicesCount),
                    SectionModel
                        .ServiceRecipients
                        .WithStatus(order.IsServiceRecipientsSectionComplete() ? "complete" : "incomplete")
                        .WithCount(serviceRecipientsCount),
                    SectionModel.CatalogueSolutions
                        .WithStatus(order.IsCatalogueSolutionsSectionComplete() ? "complete" : "incomplete")
                        .WithCount(catalogueSolutionsCount),
                    SectionModel.AdditionalServices
                        .WithStatus(order.IsAdditionalServicesSectionComplete() ? "complete": "incomplete")
                        .WithCount(additionalServicesCount),
                    SectionModel.FundingSource.WithStatus(order.IsFundingSourceComplete() ? "complete" : "incomplete")
                },
                SectionStatus = order.IsSectionStatusComplete() ? "complete" : "incomplete",
                Status = order.OrderStatus.Name
            };

            return Ok(orderSummaryModel);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<ErrorResponseModel>> CreateOrderAsync(CreateOrderModel order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            var result = await _createOrderService.CreateAsync(new CreateOrderRequest
            {
                LastUpdatedByName = User.GetUserName(),
                LastUpdatedById = User.GetUserId(),
                Description = order.Description,
                OrganisationId = order.OrganisationId,
            });

            var createOrderResponse = new ErrorResponseModel();

            if (result.IsSuccess)
            {
                createOrderResponse.OrderId = result.Value;
                return CreatedAtAction(nameof(CreateOrderAsync).TrimAsync(), null, new { orderId = result.Value }, createOrderResponse);
            }

            createOrderResponse.Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field));
            return BadRequest(createOrderResponse);
        }

        [HttpDelete]
        [Route("{orderId}")]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult> DeleteOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order is null || order.IsDeleted)
            {
                return NotFound();
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            order.IsDeleted = true;

            var name = User.Identity.Name;
            order.SetLastUpdatedBy(User.GetUserId(), name);
            await _orderRepository.UpdateOrderAsync(order);
            return NoContent();
        }

        private static readonly IDictionary<OrderStatus, Func<Order, Guid, string, Result>> _updateOrderStatusActionFactory = new Dictionary<OrderStatus, Func<Order, Guid, string, Result>>
        {
            { OrderStatus.Complete, ((order, lastUpdatedBy, lastUpdatedByName) => order.Complete(lastUpdatedBy, lastUpdatedByName)) }
        };

        [HttpPut]
        [Route("{orderId}/status")]
        public async Task<ActionResult<ErrorResponseModel>> UpdateStatusAsync(string orderId, StatusModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var orderStatus = OrderStatus.FromName(model.Status);
            if (orderStatus is null ||
                !_updateOrderStatusActionFactory.TryGetValue(orderStatus, out var updateOrderStatusFunction))
            {
                return BadRequest(new ErrorResponseModel
                {
                    Errors = new[] { ErrorMessages.InvalidOrderStatus() }
                });
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

            var updateOrderStatusResult = updateOrderStatusFunction(order, User.GetUserId(), User.GetUserName());
            if (!updateOrderStatusResult.IsSuccess)
                return BadRequest(new ErrorResponseModel
                {
                    Errors = updateOrderStatusResult.Errors.Select(error => new ErrorModel(error.Id, error.Field))
                });

            await _orderRepository.UpdateOrderAsync(order);

            return NoContent();
        }
    }
}
