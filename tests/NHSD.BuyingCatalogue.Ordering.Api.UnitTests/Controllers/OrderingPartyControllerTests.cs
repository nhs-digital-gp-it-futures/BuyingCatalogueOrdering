using System;
using System.Diagnostics.CodeAnalysis;
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
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
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
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyController controller)
        {
            order.OrderingParty.Should().NotBeNull();
            order.OrderingPartyContact.Should().NotBeNull();

            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);

            var expectedValue = new OrderingPartyModel(order.OrderingParty, order.OrderingPartyContact);

            var response = await controller.GetAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_InvokesGetOrder(
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            await controller.GetAsync(callOffId);

            service.Verify(o => o.GetOrder(callOffId));
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
            [Frozen] Mock<IOrderingPartyService> service,
            CallOffId callOffId,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync((Order)null);

            var response = await controller.UpdateAsync(callOffId, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesOrderingParty(
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            order.OrderingParty.Should().NotBeEquivalentTo(model);
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.SetOrderingParty(order, It.IsAny<OrderingParty>(), It.IsAny<Contact>())).Callback(() =>
            {
                order.OrderingParty.Name = model.Name;
                order.OrderingParty.OdsCode = model.OdsCode;
            });

            await controller.UpdateAsync(callOffId, model);
            order.OrderingParty.Should().BeEquivalentTo(
                model,
                o => o.Including(p => p.Name).Including(p => p.OdsCode));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesAddOrUpdateAddress(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            var originalAddress = order.OrderingParty.Address;

            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.SetOrderingParty(order, It.IsAny<OrderingParty>(), It.IsAny<Contact>())).Callback(() =>
            {
                order.OrderingParty.Name = model.Name;
                order.OrderingParty.OdsCode = model.OdsCode;
            });
            contactDetailsService.Setup(o => o.AddOrUpdateAddress(It.IsAny<Address>(), It.IsAny<AddressModel>()))
                .Returns(order.OrderingParty.Address);

            await controller.UpdateAsync(callOffId, model);
            contactDetailsService.Verify(s => s.AddOrUpdateAddress(
                It.Is<Address>(a => a == originalAddress),
                It.Is<AddressModel>(a => a == model.Address)));

            order.OrderingParty.Address.Should().BeEquivalentTo(originalAddress);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesAddOrUpdatePrimaryContact(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            var originalContact = order.OrderingPartyContact;

            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.SetOrderingParty(order, It.IsAny<OrderingParty>(), It.IsAny<Contact>())).Callback(() =>
            {
                order.OrderingParty.Name = model.Name;
                order.OrderingParty.OdsCode = model.OdsCode;
            });
            contactDetailsService.Setup(o => o.AddOrUpdatePrimaryContact(
                    It.Is<Contact>(c => c == originalContact),
                    It.Is<PrimaryContactModel>(c => c == model.PrimaryContact)))
                .Returns(order.OrderingPartyContact);

            await controller.UpdateAsync(callOffId, model);
            contactDetailsService.Verify(s => s.AddOrUpdatePrimaryContact(
                It.Is<Contact>(c => c == originalContact),
                It.Is<PrimaryContactModel>(c => c == model.PrimaryContact)));

            order.OrderingPartyContact.Should().BeEquivalentTo(originalContact);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.SetOrderingParty(order, It.IsAny<OrderingParty>(), It.IsAny<Contact>())).Callback(() =>
            {
                order.OrderingParty.Name = model.Name;
                order.OrderingParty.OdsCode = model.OdsCode;
            });

            var result = await controller.UpdateAsync(callOffId, model);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesSetOrderingParty(
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyModel model,
            OrderingPartyController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.SetOrderingParty(order, It.IsAny<OrderingParty>(), It.IsAny<Contact>())).Verifiable();

            await controller.UpdateAsync(callOffId, model);

            service.Verify(o => o.SetOrderingParty(order, It.IsAny<OrderingParty>(), It.IsAny<Contact>()), () => Times.AtMost(1));
        }
    }
}
