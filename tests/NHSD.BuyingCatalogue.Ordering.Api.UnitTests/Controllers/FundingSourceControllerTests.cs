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
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    public sealed class FundingSourceControllerTests
    {
        [Test]
        public void Constructor_NullOrderRepository_ThrowsException()
        {
            var builder = FundingSourceControllerBuilder.Create()
                .WithOrderRepository(null);

            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }

        [Test]
        public void Put_NullModel_ThrowsException()
        {
            var context = FundingSourceControllerTestContext.Setup();
            Assert.ThrowsAsync<ArgumentNullException>(() => context.Controller.PutFundingSource("123", null));
        }

        [Test]
        public async Task Put_OtherOrganisationId_ReturnsForbidResult()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup(organisationId);
            var order = OrderBuilder.Create().WithOrganisationId(Guid.NewGuid()).Build();
            context.Order = order;
            var controller = context.Controller;

            var response = await controller.PutFundingSource(orderId, new UpdateFundingSourceModel());
            response.Should().BeEquivalentTo(new ForbidResult());
        }

        [Test]
        public async Task Put_NullOrderReturned_ReturnsNotFound()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup(organisationId);
            context.Order = null;
            var controller = context.Controller;

            var response = await controller.PutFundingSource(orderId, new UpdateFundingSourceModel());
            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Put_AllValid_CallsUpdateOnOrderRepository(bool fundingSource)
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = FundingSourceControllerTestContext.Setup(organisationId);
            var controller = context.Controller;
            var model = new UpdateFundingSourceModel {OnlyGMS = fundingSource};
            var response = await controller.PutFundingSource(orderId, model);
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
            var controller = context.Controller;
            var model = new UpdateFundingSourceModel { OnlyGMS = true };
            var response = await controller.PutFundingSource(orderId, model);
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
                Name = "Test User";
                NameIdentity = Guid.NewGuid();
                OrderRepositoryMock = new Mock<IOrderRepository>();
                
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                Order = OrderBuilder.Create()
                    .WithOrganisationId(primaryOrganisationId)
                    .WithLastUpdatedBy(Guid.NewGuid())
                    .WithLastUpdatedByName("Gandalf the Gray")
                    .Build();

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Ordering", "Manage"),
                        new Claim("primaryOrganisationId", primaryOrganisationId.ToString()),
                        new Claim(ClaimTypes.Name, Name),
                        new Claim(ClaimTypes.NameIdentifier, NameIdentity.ToString())
                    }, "mock"));

                Controller = FundingSourceControllerBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .Build();

                Controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                };
            }

            internal string Name { get; }

            internal Guid NameIdentity { get; }

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
