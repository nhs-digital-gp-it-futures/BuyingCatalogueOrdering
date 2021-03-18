using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderingPartyControllerTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderingPartyController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            OrderingPartyController controller)
        {
            var response = await controller.GetAsync(callOffId);

            response.Should().NotBeNull();
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderExists_ReturnsTheOrderingParty(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyController controller)
        {
            order.OrderingParty.Should().NotBeNull();
            order.OrderingPartyContact.Should().NotBeNull();

            context.Add(order);
            await context.SaveChangesAsync();

            var expectedValue = new OrderingPartyModel(order.OrderingParty, order.OrderingPartyContact);

            var response = await controller.GetAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_ModelIsNull_ThrowsArgumentNullException(
            CallOffId callOffId,
            OrderingPartyController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(callOffId, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            var response = await controller.UpdateAsync(callOffId, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesOrderingParty(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            order.OrderingParty.Should().NotBeEquivalentTo(model);

            context.Order.Add(order);
            await context.SaveChangesAsync();

            await controller.UpdateAsync(callOffId, model);

            order.OrderingParty.Should().BeEquivalentTo(
                model,
                o => o.Including(p => p.Name).Including(p => p.OdsCode));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesAddOrUpdateAddress(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var originalAddress = order.OrderingParty.Address;

            await controller.UpdateAsync(callOffId, model);

            contactDetailsService.Verify(s => s.AddOrUpdateAddress(
                It.Is<Address>(a => a == originalAddress),
                It.Is<AddressModel>(a => a == model.Address)));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesAddOrUpdatePrimaryContact(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var originalContact = order.OrderingPartyContact;

            await controller.UpdateAsync(callOffId, model);

            contactDetailsService.Verify(s => s.AddOrUpdatePrimaryContact(
                It.Is<Contact>(c => c == originalContact),
                It.Is<PrimaryContactModel>(c => c == model.PrimaryContact)));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SavesToDb(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            await controller.UpdateAsync(callOffId, model);

            context.Set<Order>().First(o => o.Equals(order)).OrderingParty.Should().BeEquivalentTo(
                model,
                o => o.Including(p => p.Name).Including(p => p.OdsCode));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await controller.UpdateAsync(callOffId, model);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
