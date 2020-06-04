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
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    public sealed class CatalogueSolutionsControllerTest
    {
        [Test]
        public async Task GetAllAsync_WithCommencementDate_ReturnsOkResult()
        {
            var expectedDescription = "A description";
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.SetDescription(OrderDescription.Create(expectedDescription).Value);
            var result = await context.Controller.GetAllAsync("myOrder");
            result.Value.Should().BeOfType<CatalogueSolutionsModel>();
            var model = result.Value;
            model.CatalogueSolutions.Should().BeEmpty();
            model.OrderDescription.Should().Be(expectedDescription);
        }

        [Test]
        public async Task GetAllAsync_OrderNotFound_ReturnsNotFound()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = null;
            var result = await context.Controller.GetAllAsync("myOrder");
            result.Should().BeEquivalentTo(new ActionResult<CatalogueSolutionsModel>(new NotFoundResult()));
        }

        [Test]
        public async Task GetAllAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.OrganisationId = Guid.NewGuid();
            context.Order.CommencementDate = DateTime.Now;
            var result = await context.Controller.GetAllAsync("myOrder");
            result.Should().BeEquivalentTo(new ActionResult<CatalogueSolutionsModel>(new ForbidResult()));
        }

        [Test]
        public void Ctor_NullRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new CatalogueSolutionsController(null));
        }

        private sealed class CatalogueSolutionsControllerTestContext
        {
            private CatalogueSolutionsControllerTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                Order = new Order { OrganisationId = PrimaryOrganisationId };
                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }, "mock"));

                Controller = new CatalogueSolutionsController(OrderRepositoryMock.Object)
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

            internal CatalogueSolutionsController Controller { get; }

            internal static CatalogueSolutionsControllerTestContext Setup()
            {
                return new CatalogueSolutionsControllerTestContext();
            }
        }
    }
}
