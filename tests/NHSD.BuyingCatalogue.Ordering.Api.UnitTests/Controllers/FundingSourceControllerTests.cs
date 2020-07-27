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
            var builder = FundingSourceControllerBuilder.Create()
                .WithOrderRepository(null);

            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }

        [Test]
        public async Task GetAsync_OrderDoesNotExist_ReturnsNotFound()
        {
            const string orderId = "DOESNOTEXIST";
            var context = FundingSourceControllerTestContext.Setup();
            context.Order = null;

            var result = await context.Controller.GetAsync(orderId);

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

            var actual = await context.Controller.GetAsync(orderId);

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

            var actual = await context.Controller.GetAsync(orderId);

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

            await context.Controller.GetAsync(orderId);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public void Put_NullModel_ThrowsException()
        {
            var context = FundingSourceControllerTestContext.Setup();
            Assert.ThrowsAsync<ArgumentNullException>(() => context.Controller.PutFundingSourceAsync("123", null));
        }

        [Test]
        public async Task Put_OtherOrganisationId_ReturnsForbidResult()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup(organisationId);
            var order = OrderBuilder.Create().WithOrganisationId(Guid.NewGuid()).Build();
            context.Order = order;

            var response = await context.Controller.PutFundingSourceAsync(orderId, new UpdateFundingSourceModel());
            response.Should().BeEquivalentTo(new ForbidResult());
        }

        [Test]
        public async Task Put_NullOrderReturned_ReturnsNotFound()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup(organisationId);
            context.Order = null;

            var response = await context.Controller.PutFundingSourceAsync(orderId, new UpdateFundingSourceModel());
            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Put_AllValid_CallsUpdateOnOrderRepository(bool fundingSource)
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup(organisationId);
            var model = new UpdateFundingSourceModel {OnlyGMS = fundingSource};

            var response = await context.Controller.PutFundingSourceAsync(orderId, model);
            response.Should().BeEquivalentTo(new NoContentResult());

            context.OrderRepositoryMock.Verify(x =>
                x.UpdateOrderAsync(It.Is<Order>(
                    y => y.FundingSourceOnlyGMS == fundingSource)));
        }
        
        [Test]
        public async Task Put_AllValid_UpdatesLastUpdated()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup(organisationId);
            var model = new UpdateFundingSourceModel { OnlyGMS = true };

            var response = await context.Controller.PutFundingSourceAsync(orderId, model);
            response.Should().BeEquivalentTo(new NoContentResult());

            context.OrderRepositoryMock.Verify(x =>
                x.UpdateOrderAsync(It.Is<Order>(
                    y => y.LastUpdatedBy == context.NameIdentity &&
                         y.LastUpdatedByName == context.Name)));
        }

        private sealed class FundingSourceControllerTestContext
        {
            private FundingSourceControllerTestContext(Guid primaryOrganisationId)
            {
                PrimaryOrganisationId = primaryOrganisationId;
                Name = "Test User";
                NameIdentity = Guid.NewGuid();
                
				Order = OrderBuilder.Create()
                    .WithOrganisationId(PrimaryOrganisationId)
                    .WithLastUpdatedBy(Guid.NewGuid())
                    .WithLastUpdatedByName("Gandalf the Gray")
                    .Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, Name),
                    new Claim(ClaimTypes.NameIdentifier, NameIdentity.ToString())
                }, "mock"));

                Controller = FundingSourceControllerBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithControllerContext(
                        new ControllerContext
                        {
                            HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                        })
                    .Build();
            }

            internal string Name { get; }

            internal Guid NameIdentity { get; }

            internal Guid PrimaryOrganisationId { get; set; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal FundingSourceController Controller { get; }
            
            internal Order Order { get; set; }

            internal static FundingSourceControllerTestContext Setup()
            {
                return new FundingSourceControllerTestContext(Guid.NewGuid());
            }

            internal static FundingSourceControllerTestContext Setup(Guid primaryOrganisationId)
            {
                return new FundingSourceControllerTestContext(primaryOrganisationId);
            }
        }
    }
}
