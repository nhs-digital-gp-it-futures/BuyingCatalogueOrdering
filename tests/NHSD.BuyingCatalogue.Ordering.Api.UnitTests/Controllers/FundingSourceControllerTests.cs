using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class FundingSourceControllerTests
    {
        [Test]
        public void Constructor_NullOrderRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FundingSourceControllerBuilder.Create().WithOrderRepository(null).Build());
        }

        [Test]
        public async Task GetAsync_OrderDoesNotExist_ReturnsNotFound()
        {
            const string orderId = "DOESNOTEXIST";
            var context = FundingSourceControllerTestContext.Setup();
            using var controller = context.FundingSourceController;

            var result = await controller.GetAsync(orderId);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetAsync_DifferentOrganisationId_ForbiddenReturned()
        {
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup();
            context.PrimaryOrganisationId = Guid.NewGuid();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            using var controller = context.FundingSourceController;
            var actual = await controller.GetAsync(orderId);

            actual.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task GetAsync_OrderExists_FundingSourceDetailsReturned()
        {
            const string orderId = "C0000014-01";

            var context = FundingSourceControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithFundingSourceOnlyGms(true)
                .Build();

            using var controller = context.FundingSourceController;
            var actual = await controller.GetAsync(orderId);

            var expected = new GetFundingSourceModel
            {
                OnlyGMS = context.Order.FundingSourceOnlyGMS
            };

            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetAsync_GetOrderByIdAsync_CalledOnce()
        {
            const string orderId = "C0000014-01";

            var context = FundingSourceControllerTestContext.Setup();

            using var controller = context.FundingSourceController;

            await controller.GetAsync(orderId);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        private sealed class FundingSourceControllerTestContext
        {
            private FundingSourceControllerTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }, "mock"));

                FundingSourceController = FundingSourceControllerBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithControllerContext(
                        new ControllerContext
                        {
                            HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                        })
                    .Build();
            }

            internal Guid PrimaryOrganisationId { get; set; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Order Order { get; set; }

            internal FundingSourceController FundingSourceController { get; }

            internal static FundingSourceControllerTestContext Setup()
            {
                return new FundingSourceControllerTestContext();
            }
        }
    }
}
