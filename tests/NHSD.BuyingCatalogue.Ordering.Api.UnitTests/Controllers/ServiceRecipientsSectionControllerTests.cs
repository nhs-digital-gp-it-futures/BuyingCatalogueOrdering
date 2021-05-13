using System.Collections.Generic;
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
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class ServiceRecipientsSectionControllerTests
    {
        [Test]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceRecipientsSectionController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllAsync_OrderDoesNotExist_ReturnsNotFound(
            [Frozen] Mock<IServiceRecipientService> service,
            CallOffId callOffId,
            ServiceRecipientsSectionController controller)
        {
            service.Setup(s => s.GetAllOrderItemRecipients(callOffId)).ReturnsAsync((List<ServiceRecipient>)null);
            var response = await controller.GetAllAsync(callOffId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllAsync_NoServiceRecipient_ReturnsEmptyList(
            [Frozen] Mock<IServiceRecipientService> service,
            [Frozen] CallOffId callOffId,
            ServiceRecipientsSectionController controller)
        {
            service.Setup(s => s.GetAllOrderItemRecipients(callOffId)).ReturnsAsync(new List<ServiceRecipient>());

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = new List<ServiceRecipientModel>(),
            };

            var response = await controller.GetAllAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllAsync_HasServiceRecipients_ReturnsExpectedResult(
            [Frozen] Mock<IServiceRecipientService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            List<OrderItem> orderItems,
            ServiceRecipientsSectionController controller)
        {
            foreach (var orderItem in orderItems)
            {
                order.AddOrUpdateOrderItem(orderItem);
            }

            var serviceRecipients = order.OrderItems
                .Where(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                .SelectMany(o => o.OrderItemRecipients)
                .Select(r => new ServiceRecipient(r.Recipient.OdsCode, r.Recipient.Name))
                .ToList();

            service.Setup(s => s.GetAllOrderItemRecipients(callOffId)).ReturnsAsync(
                serviceRecipients);

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients.Select(r =>
                    new ServiceRecipientModel { Name = r.Name, OdsCode = r.OdsCode })
                    .ToList(),
            };

            var response = await controller.GetAllAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expected);
        }
    }
}
