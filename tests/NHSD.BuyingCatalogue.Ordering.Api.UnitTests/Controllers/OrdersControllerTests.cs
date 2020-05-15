﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrdersControllerTests
    {
        [Test]
        public void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrdersController(null , null);
            });
        }

        [Test]
        public async Task GetAllAsync_NoOrdersExist_ReturnsEmptyResult()
        {
            var context = OrdersControllerTestContext.Setup();

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(Guid.Empty) as OkObjectResult;
            var orders = result.Value as List<OrderModel>;
            orders.Should().BeEmpty();
        }

        [TestCase(null, "Some Description")]
        [TestCase("C0000014-01", "Some Description")]
        public async Task GetAllAsync_SingleOrderWithOrganisationIdExists_ReturnsTheOrder(string orderId, string orderDescription)
        {
            var organisationId = Guid.NewGuid();

            var orders = new List<(Order order, OrderModel expected)>
            {
                CreateOrderTestData(orderId, organisationId, orderDescription)
            };

            var context = OrdersControllerTestContext.Setup();
            context.Orders = orders.Select(x => x.order);

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(organisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderModel>;
            ordersResult.Should().ContainSingle();
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public async Task GetAllAsync_MultipleOrdersWithOrganisationIdExist_ReturnsAllOrders()
        {
            var organisationId = Guid.NewGuid();

            var orders = new List<(Order order, OrderModel expected)>
            {
                CreateOrderTestData("C0000014-01", organisationId, "Some Description"),
                CreateOrderTestData("C000012-01", organisationId, "Another Description")
            };

            var context = OrdersControllerTestContext.Setup();
            context.Orders = orders.Select(x => x.order);

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(organisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderModel>;
            ordersResult.Count.Should().Be(2);
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public async Task GetAll_OrdersByOrganisationId_CalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();

            using var controller = context.OrdersController;

            await controller.GetAllAsync(Guid.Empty);

            context.OrderRepositoryMock.Verify(x => x.ListOrdersByOrganisationIdAsync(Guid.Empty), Times.Once);
        }

        [Test]
        public async Task GetOrderSummaryAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var context = OrdersControllerTestContext.Setup();

            using var controller = context.OrdersController;

            var response = await controller.GetOrderSummaryAsync("INVALID");
            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task GetOrderSummaryAsync_IsSummaryComplete_ReturnResult()
        {
            const string orderId = "C0000014-01";

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description");

            var context = OrdersControllerTestContext.Setup();
            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        private static (Order order, OrderModel expectedOrder) CreateOrderTestData(string orderId, Guid organisationId, string description)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .WithDescription(description)
                .Build();

            return (order: repositoryOrder,
                expectedOrder: new OrderModel
                {
                    OrderId = repositoryOrder.OrderId,
                    Description = repositoryOrder.Description.Value,
                    Status = repositoryOrder.OrderStatus.Name,
                    LastUpdated = repositoryOrder.LastUpdated,
                    DateCreated = repositoryOrder.Created,
                    LastUpdatedBy = repositoryOrder.LastUpdatedBy
                });
        }

        private static (Order order, OrderSummaryModel expectedSummary) CreateOrderSummaryTestData(string orderId, string description)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithDescription(description)
                .Build();

            return (order: repositoryOrder,
                expectedSummary: new OrderSummaryModel
                {
                    OrderId = repositoryOrder.OrderId,
                    OrganisationId = repositoryOrder.OrganisationId,
                    Description = repositoryOrder.Description.Value,
                    Sections = new List<SectionModel>
                    {
                        new SectionModel
                        {
                            Id = "ordering-description",
                            Status = string.IsNullOrWhiteSpace(repositoryOrder.Description.Value) ? "incomplete" : "complete"
                        },
                        new SectionModel {Id = "ordering-party", Status = "incomplete"},
                        new SectionModel {Id = "supplier", Status = "incomplete"},
                        new SectionModel {Id = "commencement-date", Status = "incomplete"},
                        new SectionModel {Id = "associated-services", Status = "incomplete"},
                        new SectionModel {Id = "service-recipients", Status = "incomplete"},
                        new SectionModel {Id = "catalogue-solutions", Status = "incomplete"},
                        new SectionModel {Id = "additional-services", Status = "incomplete"},
                        new SectionModel {Id = "funding-source", Status = "incomplete"}
                    }
                });
        }

        private sealed class OrdersControllerTestContext
        {
            private OrdersControllerTestContext()
            {
                OrderRepositoryMock = new Mock<IOrderRepository>();

                CreateOrderServiceMock = new Mock<ICreateOrderService>();

                Orders = new List<Order>();
                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);

                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                OrdersController = new OrdersController(OrderRepositoryMock.Object, CreateOrderServiceMock.Object);
            }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<ICreateOrderService> CreateOrderServiceMock { get; }

            internal IEnumerable<Order> Orders { get; set; }

            internal Order Order { get; set; }

            internal OrdersController OrdersController { get; }

            internal static OrdersControllerTestContext Setup()
            {
                return new OrdersControllerTestContext();
            }
        }
    }
}
