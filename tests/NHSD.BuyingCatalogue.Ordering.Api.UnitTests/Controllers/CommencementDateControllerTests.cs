using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    internal sealed class CommencementDateControllerTests
    {
        [Test]
        public async Task Update_ValidDateTime_UpdatesAndReturnsNoContent()
        {
            var context = CommencementDateControllerTestContext.Setup();
            var model = new CommencementDateModel {CommencementDate = DateTime.Now};
            var result = await context.Controller.Update("myOrder", model);
            context.OrderRepositoryMock.Verify(x => x.UpdateOrderAsync(It.Is<Order>(order => order.CommencementDate == model.CommencementDate)));
            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public void Update_NullModel_ThrowsException()
        {
            var context = CommencementDateControllerTestContext.Setup();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await context.Controller.Update("myOrder", null));
        }

        [Test]
        public void Update_NullDateTime_ThrowsException()
        {
            var context = CommencementDateControllerTestContext.Setup();
            var model = new CommencementDateModel { CommencementDate = null };
            Assert.ThrowsAsync<ArgumentException>(async () => await context.Controller.Update("myOrder", model));
        }

        [Test]
        public async Task Update_UserHasDifferentPrimaryOrganisationId_ReturnsForbidden()
        {
            var context = CommencementDateControllerTestContext.Setup();
            context.Order.OrganisationId = Guid.NewGuid();
            var model = new CommencementDateModel { CommencementDate = DateTime.Now };
            var result = await context.Controller.Update("myOrder", model);
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task Update_NoOrderFound_ReturnsNotFound()
        {
            var context = CommencementDateControllerTestContext.Setup();
            context.Order = null;
            var model = new CommencementDateModel { CommencementDate = DateTime.Now };
            var result = await context.Controller.Update("myOrder", model);
            result.Should().BeOfType<NotFoundResult>();
        }

        private sealed class CommencementDateControllerTestContext
        {
            private CommencementDateControllerTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                Order = new Order {OrganisationId = PrimaryOrganisationId};
                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }, "mock"));

                Controller = new CommencementDateController(OrderRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                    }
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Order Order { get; set; }

            internal CommencementDateController Controller { get; }

            internal static CommencementDateControllerTestContext Setup()
            {
                return new CommencementDateControllerTestContext();
            }
        }
    }
}
