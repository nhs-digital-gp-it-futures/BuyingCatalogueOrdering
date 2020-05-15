using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    internal sealed class OrderDescriptionControllerTests
    {
        [Test]
        public void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrderDescriptionController(null);
            });
        }

        [TestCase(null)]
        public async Task Get_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var context = OrderDescriptionTestContext.Setup();

            using var controller = context.OrderDescriptionController;

            var response = await controller.GetAsync("INVALID");

            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task Get_OrderIdExists_ReturnsTheOrdersDescription()
        {
            var orderId = "C0000014-01";

            var testData = CreateOrderDescriptionTestData(orderId, OrderDescription.Create("Test Description").Value);

            var context = OrderDescriptionTestContext.Setup();
            context.Order = testData.order;

            using var controller = context.OrderDescriptionController;

            var result = await controller.GetAsync(orderId) as OkObjectResult;

            var orderDescription = result.Value as OrderDescriptionModel;
            orderDescription.Should().BeEquivalentTo(testData.expectedDescription);
        }

        [Test]
        public async Task Get_OrderById_CalledOnce()
        {
            var context = OrderDescriptionTestContext.Setup();

            using var controller = context.OrderDescriptionController;

            await controller.GetAsync(string.Empty);
            
            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(string.Empty), Times.Once);
        }

        [TestCase(null)]
        [TestCase("INVALID")]
        public async Task UpdateAsync_OrderIdDoesNotExist_ReturnNotFound(string orderId)
        {
            var context = OrderDescriptionTestContext.Setup();

            using var controller = context.OrderDescriptionController;

            var response =
                await controller.UpdateAsync(orderId, new OrderDescriptionModel {Description = "Desc"});

            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task UpdateAsync_ModelIsNull_ReturnNotFound()
        {
            const string orderId = "C0000014-01";

            var testData = CreateOrderDescriptionTestData(orderId, OrderDescription.Create("Test Description").Value);

            var context = OrderDescriptionTestContext.Setup();
            context.Order = testData.order;

            using var controller = context.OrderDescriptionController;

            var response =
                await controller.UpdateAsync(orderId, null);

            response.Should().BeEquivalentTo(new NotFoundResult());
        }
        
        [Test]
        public async Task UpdateAsync_UpdatedDescriptionIsValid_ReturnsNoContent()
        {
            const string orderId = "C0000014-01";

            var testData = CreateOrderDescriptionTestData(orderId, OrderDescription.Create("Test Description").Value);

            var context = OrderDescriptionTestContext.Setup();
            context.Order = testData.order;

            using var controller = context.OrderDescriptionController;

            var response =
                await controller.UpdateAsync(orderId,
                    new OrderDescriptionModel {Description = "New Description"});

            response.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task UpdateAsync_UpdateAndGet_CalledOnce()
        {
            const string orderId = "C0000014-01";
            var newDescription = OrderDescription.Create("New Description").Value;

            var testData = CreateOrderDescriptionTestData(orderId, OrderDescription.Create("Test Description").Value);

            var context = OrderDescriptionTestContext.Setup();
            context.Order = testData.order;

            using var controller = context.OrderDescriptionController;

            await controller.UpdateAsync(orderId,
                    new OrderDescriptionModel { Description = newDescription.Value });

            var updatedOrder = testData.order;
            updatedOrder.SetDescription(newDescription);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
            context.OrderRepositoryMock.Verify(x => x.UpdateOrderAsync(updatedOrder), Times.Once);
        }

        private static (Order order, OrderDescriptionModel expectedDescription) CreateOrderDescriptionTestData(
            string orderId, OrderDescription description)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithDescription(description.Value)
                .Build();

            return (order: repositoryOrder,
                expectedDescription: new OrderDescriptionModel() {Description = repositoryOrder.Description.Value});
        }

        private sealed class OrderDescriptionTestContext
        {
            private OrderDescriptionTestContext()
            {
                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                OrderDescriptionController = new OrderDescriptionController(OrderRepositoryMock.Object);
            }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Order Order { get; set; }

            internal OrderDescriptionController OrderDescriptionController { get; }

            internal static OrderDescriptionTestContext Setup()
            {
                return new OrderDescriptionTestContext();
            }
        }
    }
}
