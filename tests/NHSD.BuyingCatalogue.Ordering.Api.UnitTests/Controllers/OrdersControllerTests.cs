using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrdersControllerTests
    {
        private readonly OrderStatus submittedStatus = new OrderStatus { Name = "Submitted", OrderStatusId = 1 };
        private readonly OrderStatus unSubmittedStatus = new OrderStatus { Name = "unSubmitted", OrderStatusId = 2 };

        [Test]
        public void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrdersController(null);
            });
        }

        [Test]
        public async Task GetAllAsync_NoOrdersExist_ReturnsEmptyResult()
        {
            using var controller = OrdersControllerBuilder
                .Create()
                .WithListOrders(new List<Order>())
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBeNull();

            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<List<OrderModel>>();

            var ordersResult = objectResult.Value as List<OrderModel>;
            ordersResult.Count().Should().Be(0);
        }

        [TestCase(null, null)]
        [TestCase("C0000014-01", "Some Description")]
        [TestCase("C0000014-02", null)]
        public async Task GetAllAsync_SingleOrderExists_ReturnsTheOrder(string orderId, string orderDescription)
        {
            var model = new List<Order>()
            {
                new Order()
                {
                    OrderId = orderId,
                    Description = orderDescription,
                    LastUpdatedBy = Guid.NewGuid(),
                    LastUpdated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    OrderStatus = submittedStatus
                }
            };

            var expected = new List<OrderModel>()
            {
                new OrderModel()
                {
                    OrderId = model[0].OrderId,
                    OrderDescription = model[0].Description,
                    LastUpdatedBy = model[0].LastUpdatedBy,
                    LastUpdated = model[0].LastUpdated,
                    DateCreated = model[0].Created,
                    Status = model[0].OrderStatus.Name
                }
            };


            using var controller = OrdersControllerBuilder.Create().WithListOrders(model).Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBeNull();

            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<List<OrderModel>>();

            var ordersResult = objectResult.Value as List<OrderModel>;
            ordersResult.Count().Should().Be(1);
            ordersResult.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetAllAsync_MultipleOrdersExist_ReturnsAllOrders()
        {
            var model = new List<Order>()
            {
                new Order()
                {
                    OrderId = "C0000014-01",
                    Description = "Some Description",
                    LastUpdatedBy = Guid.NewGuid(),
                    LastUpdated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    OrderStatus = submittedStatus
                },
                new Order()
                {
                    OrderId = "C000012-01",
                    Description = "Another Description",
                    LastUpdatedBy = Guid.NewGuid(),
                    LastUpdated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    OrderStatus = unSubmittedStatus
                }
            };

            var expected = new List<OrderModel>()
            {
                new OrderModel()
                {
                    OrderId = model[0].OrderId,
                    OrderDescription = model[0].Description,
                    LastUpdatedBy = model[0].LastUpdatedBy,
                    LastUpdated = model[0].LastUpdated,
                    DateCreated = model[0].Created,
                    Status = model[0].OrderStatus.Name
                },
                new OrderModel()
                {
                    OrderId = model[1].OrderId,
                    OrderDescription = model[1].Description,
                    LastUpdatedBy = model[1].LastUpdatedBy,
                    LastUpdated = model[1].LastUpdated,
                    DateCreated = model[1].Created,
                    Status = model[1].OrderStatus.Name
                }
            };

            using var controller = OrdersControllerBuilder.Create().WithListOrders(model).Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBeNull();

            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<List<OrderModel>>();

            var ordersResult = objectResult.Value as List<OrderModel>;
            ordersResult.Count().Should().Be(2);
            ordersResult.Should().BeEquivalentTo(expected);
        }
    }
}
