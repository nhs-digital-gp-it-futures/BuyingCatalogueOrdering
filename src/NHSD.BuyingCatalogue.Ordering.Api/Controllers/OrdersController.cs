using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;

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
        [Route("/api/v1/orders/{orderId}")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static endpoints make the endpoint unreachable")]
        public OrderModel Get()
        {
            return new OrderModel
            {
                Description = "Hello",
                OrderParty = new OrderingPartyModel
                {
                    Name = "NHS Test CCG",
                    OdsCode = "08E",
                    Address = new AddressModel
                    {
                        Line1 = "1 Cool Street",
                        Town = "Yoloville",
                        County = "Brillshire",
                        Country = "Megaland",
                        Postcode = "YE37 1ME"
                    },
                    PrimaryContact = new PrimaryContactModel
                    {
                        EmailAddress = "bob@bob.com",
                        FirstName = "Bob",
                        LastName = "Bobbington",
                        TelephoneNumber = "1337 331331"
                    }
                },
                CommencementDate = DateTime.Today + TimeSpan.FromDays(1),
                Supplier = new SupplierModel
                {
                    Address = new AddressModel
                    {
                        Line1 = "1 Supplier Lane",
                        Town = "Supplierville",
                        County = "Suppliershire",
                        Country = "Supplierland",
                        Postcode = "SUPP 1ER"
                    },
                    Name = "Cool Supplier",
                    PrimaryContact = new PrimaryContactModel
                    {
                        EmailAddress = "alice@alice.com",
                        FirstName = "Alice",
                        LastName = "Alicington",
                        TelephoneNumber = "12345 678910"
                    }
                },
                TotalOneOffCost = 100m,
                TotalRecurringCostPerMonth = 100m,
                TotalRecurringCostPerYear = 1200m,
                TotalOwnershipCost = 3700m,
                UnusedServiceRecipients = new []
                {
                    new ServiceRecipientModel
                    {
                        Name = "NHS Test CCG",
                        OdsCode = "08E"
                    },
                    new ServiceRecipientModel
                    {
                        Name = "Blue Mountain Medical Practice",
                        OdsCode = "A10001"
                    },
                    new ServiceRecipientModel
                    {
                        Name = "Lighthouse surgery",
                        OdsCode = "Y06616"
                    },
                    new ServiceRecipientModel
                    {
                        Name = "Lincoln Test Practice",
                        OdsCode = "Y04336"
                    }, 
                    new ServiceRecipientModel
                    {
                        Name = "Mckenzie Group Practice",
                        OdsCode = "A81070"
                    }, 
                    new ServiceRecipientModel
                    {
                        Name = "Medical Princes Centre",
                        OdsCode = "B81052"
                    }, 
                    new ServiceRecipientModel
                    {
                        Name = "NHS Test CCG",
                        OdsCode = "08E"
                    }, 
                    new ServiceRecipientModel
                    {
                        Name = "The Lynton Practice",
                        OdsCode = "B81055"
                    }, 
                    new ServiceRecipientModel
                    {
                        Name = "The Ridings Medical Group",
                        OdsCode = "B81061"
                    }, 
                    new ServiceRecipientModel
                    {
                        Name = "Trent View Medical Practice",
                        OdsCode = "B81065"
                    }
                },
                OrderItems = new[]
                {
                    // Order Items - One off cost
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-08E-23",
                        ServiceRecipientsOdsCode = "08E",
                        CataloguePriceType = "Flat",
                        CatalogueItemType = "Associated Service",
                        CatalogueItemName = "Core Training",
                        ProvisioningType = "Declarative",
                        ItemUnitDescription = "per Day",
                        Price = 585.00m,
                        Quantity = 70,
                        CostPerYear = 40850.00m
                    },
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-08E-37",
                        ServiceRecipientsOdsCode = "08E",
                        CataloguePriceType = "Tiered",
                        CatalogueItemType = "Associated Service",
                        CatalogueItemName = "George Digitisation",
                        ProvisioningType = "Declarative",
                        ItemUnitDescription = "per Record",
                        Quantity = 250,
                        CostPerYear = 887.50m
                    },
                    // Order items - recurring cost
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-A10001-1",
                        ServiceRecipientsOdsCode = "A10001",
                        CataloguePriceType = "Flat",
                        CatalogueItemType = "Solution",
                        CatalogueItemName = "Catalogue Solution name 1",
                        ProvisioningType = "Patient",
                        ItemUnitDescription = "per patient",
                        TimeUnitDescription = "per year",
                        QuantityPeriodDescription = "per month",
                        Price = 1.26m,
                        Quantity = 3415,
                        DeliveryDate = new DateTime(2021, 7, 6),
                        CostPerYear = 4302.90m
                    },
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-A10001-3",
                        ServiceRecipientsOdsCode = "A10001",
                        CataloguePriceType = "Tiered",
                        CatalogueItemType = "AdditionalService",
                        CatalogueItemName = "Additional Service 1 of Catalogue Solution name 1",
                        ProvisioningType = "Patient",
                        ItemUnitDescription = "per patient",
                        TimeUnitDescription = "per year",
                        QuantityPeriodDescription = "per month",
                        Quantity = 420,
                        CostPerYear = 3000.00m
                    },
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-Y06616-7",
                        ServiceRecipientsOdsCode = "Y06617",
                        CataloguePriceType = "Flat",
                        CatalogueItemType = "Additional Service",
                        CatalogueItemName = "Additional Service 2 of Catalogue Solution name 1",
                        ProvisioningType = "Declarative",
                        ItemUnitDescription = "per license",
                        TimeUnitDescription = "per month",
                        QuantityPeriodDescription = "per year",
                        Price = 25.05m,
                        Quantity = 4,
                        CostPerYear = 25200m
                    },
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-Y04336-8",
                        ServiceRecipientsOdsCode = "Y04336",
                        CataloguePriceType = "Tiered",
                        CatalogueItemType = "Solution",
                        CatalogueItemName = "Catalogue Solution name 2",
                        ProvisioningType = "Declarative",
                        ItemUnitDescription = "per user",
                        TimeUnitDescription = "per month",
                        QuantityPeriodDescription = "per year",
                        DeliveryDate = new DateTime(2021, 4, 24),
                        Quantity = 10,
                        CostPerYear = 3320m
                    },
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-A81070-11",
                        ServiceRecipientsOdsCode = "A81070",
                        CataloguePriceType = "Flat",
                        CatalogueItemType = "Solution",
                        CatalogueItemName = "Catalogue Solution name 1",
                        ProvisioningType = "OnDemand",
                        ItemUnitDescription = "per consultation",
                        QuantityPeriodDescription = "per month",
                        DeliveryDate = new DateTime(2021, 4, 24),
                        Price = 0.7m,
                        Quantity = 250,
                        CostPerYear = 175
                    }, 
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-B81052-13",
                        ServiceRecipientsOdsCode = "B81052",
                        CataloguePriceType = "Tiered",
                        CatalogueItemType = "Additional Service",
                        CatalogueItemName = "Additional Service 1 of Catalogue Solution name 2",
                        ProvisioningType = "OnDemand",
                        ItemUnitDescription = "per consultation",
                        QuantityPeriodDescription = "per year",
                        Quantity = 330,
                        CostPerYear = 3782.20m
                    }, 
                    new OrderItemModel
                    {
                        ItemId = "C000001-01-08E-15",
                        ServiceRecipientsOdsCode = "08E",
                        CataloguePriceType = "Flat",
                        CatalogueItemType = "Associated Service",
                        CatalogueItemName = "Associated Service name 3",
                        ProvisioningType = "OnDemand",
                        ItemUnitDescription = "per fragment",
                        QuantityPeriodDescription = "per month",
                        Price = 0.7m,
                        Quantity = 600,
                        CostPerYear = 5040m
                    } 
                }
            };
        }

        [HttpGet]
        [Route("/api/v1/organisations/{organisationId}/[controller]")]
        public async Task<ActionResult> GetAllAsync(Guid organisationId)
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
                Status = order.OrderStatus.Name
            })
                .ToList();

            return Ok(orderModelResult);
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
                    SectionModel.AssociatedServices,
                    SectionModel
                        .ServiceRecipients
                        .WithStatus(order.IsServiceRecipientsSectionComplete() ? "complete" : "incomplete")
                        .WithCount(serviceRecipientsCount),
                    SectionModel.CatalogueSolutions.WithStatus(order.IsCatalogueSolutionsSectionComplete() ? "complete" : "incomplete"),
                    SectionModel.AdditionalServices,
                    SectionModel.FundingSource
                }
            };

            return Ok(orderSummaryModel);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<CreateOrderResponseModel>> CreateOrderAsync([FromBody][Required] CreateOrderModel order)
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

            var createOrderResponse = new CreateOrderResponseModel();

            if (result.IsSuccess)
            {
                createOrderResponse.OrderId = result.Value;
                return CreatedAtAction(nameof(CreateOrderAsync).TrimAsync(), null, new { orderId = result.Value }, createOrderResponse);
            }

            createOrderResponse.Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field));
            return BadRequest(createOrderResponse);
        }
    }
}
