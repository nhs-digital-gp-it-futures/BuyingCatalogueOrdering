using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class OrdersController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetOrders()
        {
            var orders = new List<OrdersModel>
            {
                new OrdersModel
                {
                    OrderId = "C0000014-01",
                    OrderDescription = "Some Order",
                    LastUpdatedBy = "Bob Smith",
                    LastUpdated = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    Status = "Unsubmitted"
                },
                new OrdersModel
                {
                    OrderId = "C000012-01",
                    OrderDescription = "Some new order",
                    LastUpdatedBy = "Alice Smith",
                    LastUpdated = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    Status = "Submitted"
                }
            };

            return Ok(orders);
        }

        [HttpGet]
        [Route("{orderId}/summary")]
        public ActionResult GetOrderSummary(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return NotFound();

            return Ok(new OrderSummaryModel
            {
                OrderId = orderId,
                OrganisationId = Guid.Parse("B7EE5261-43E7-4589-907B-5EEF5E98C085"),
                LastUpdatedBy = "Bob Smith",
                LastUpdated = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow,
                Description = "Some description about the order.",
                Sections = new List<SectionModel>
                {
                    new SectionModel
                    {
                        Id = "ordering-description",
                        Status = "complete"
                    },
                    new SectionModel
                    {
                        Id = "ordering-party",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "supplier",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "commencement-date",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "associated-services",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "service-recipients",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "catalogue-solutions",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "additional-services",
                        Status = "incomplete"
                    },
                    new SectionModel
                    {
                        Id = "funding-source",
                        Status = "incomplete"
                    },
                }
            });
        }
    }
}
