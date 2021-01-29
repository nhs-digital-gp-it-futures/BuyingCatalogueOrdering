using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CommencementDateControllerTests
    {
        [Test]
        public static async Task Update_ValidDateTime_UpdatesAndReturnsNoContent()
        {
            var context = CommencementDateControllerTestContext.Setup();
            var model = new CommencementDateModel { CommencementDate = DateTime.Now };
            var result = await context.Controller.Update("myOrder", model);
            context.OrderRepositoryMock.Verify(r => r.UpdateOrderAsync(It.Is<Order>(order => order.CommencementDate == model.CommencementDate)));
            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public static void Update_NullModel_ThrowsException()
        {
            var context = CommencementDateControllerTestContext.Setup();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await context.Controller.Update("myOrder", null));
        }

        [Test]
        public static void Update_NullDateTime_ThrowsException()
        {
            var context = CommencementDateControllerTestContext.Setup();
            var model = new CommencementDateModel { CommencementDate = null };
            Assert.ThrowsAsync<ArgumentException>(async () => await context.Controller.Update("myOrder", model));
        }

        [Test]
        public static async Task Update_UserHasDifferentPrimaryOrganisationId_ReturnsForbidden()
        {
            var context = CommencementDateControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var model = new CommencementDateModel { CommencementDate = DateTime.Now };
            var result = await context.Controller.Update("myOrder", model);
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static async Task Update_NoOrderFound_ReturnsNotFound()
        {
            var context = CommencementDateControllerTestContext.Setup();
            context.Order = null;
            var model = new CommencementDateModel { CommencementDate = DateTime.Now };
            var result = await context.Controller.Update("myOrder", model);
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestCase("01/20/2012")]
        [TestCase(null)]
        public static async Task GetAsync_WithCommencementDate_ReturnsOkResult(DateTime? commencementDate)
        {
            var context = CommencementDateControllerTestContext.Setup();
            context.Order.CommencementDate = commencementDate;

            var result = await context.Controller.GetAsync("myOrder");

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeOfType<CommencementDateModel>();
            result.As<OkObjectResult>().Value.As<CommencementDateModel>()
                .CommencementDate.Should().Be(context.Order.CommencementDate);
        }

        [Test]
        public static async Task GetAsync_OrderNotFound_ReturnsNotFound()
        {
            var context = CommencementDateControllerTestContext.Setup();
            context.Order = null;
            var result = await context.Controller.GetAsync("myOrder");
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = CommencementDateControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            context.Order.CommencementDate = DateTime.Now;
            var result = await context.Controller.GetAsync("myOrder");
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static void Constructor_NullRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CommencementDateController(null));
        }

        private sealed class CommencementDateControllerTestContext
        {
            private CommencementDateControllerTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                Order = OrderBuilder
                    .Create()
                    .WithOrganisationId(PrimaryOrganisationId)
                    .Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(r => r.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Ordering", "Manage"),
                        new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                        new Claim(ClaimTypes.Name, "Test User"),
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    },
                    "mock"));

                Controller = new CommencementDateController(OrderRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal },
                    },
                };
            }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Order Order { get; set; }

            internal CommencementDateController Controller { get; }

            private Guid PrimaryOrganisationId { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal static CommencementDateControllerTestContext Setup() => new();
        }
    }
}
