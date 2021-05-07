using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
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
            service.Setup(s => s.GetAllOrderItemRecipient(callOffId)).ReturnsAsync((List<ServiceRecipient>)null);
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
            service.Setup(s => s.GetAllOrderItemRecipient(callOffId)).ReturnsAsync(new List<ServiceRecipient>());

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = new List<ServiceRecipientModel>(),
            };

            var response = await controller.GetAllAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllAsync_HasSelectedServiceRecipients_ReturnsExpectedResult(
            [Frozen] Mock<IServiceRecipientService> service,
            [Frozen] CallOffId callOffId,
            IReadOnlyList<SelectedServiceRecipient> serviceRecipients,
            Order order,
            ServiceRecipientsSectionController controller)
        {
            order.SetSelectedServiceRecipients(serviceRecipients);

            service.Setup(s => s.GetAllOrderItemRecipient(callOffId)).ReturnsAsync(
                serviceRecipients.Select(s => new ServiceRecipient(s.Recipient.OdsCode, s.Recipient.Name)).ToList());

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients.Select(r => new ServiceRecipientModel(r)).ToList(),
            };

            var response = await controller.GetAllAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_ModelIsNull_ThrowsArgumentNullException(
            ServiceRecipientsSectionController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(default, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_OrderDoesNotExist_ReturnsNotFound(
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            var response = await controller.UpdateAsync(default, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesAddOrUpdateServiceRecipients(
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>());

            serviceRecipientService.Setup(s => s.GetOrder(callOffId)).ReturnsAsync(order);
            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);

            await controller.UpdateAsync(callOffId, model);

            serviceRecipientService.Verify(addOrUpdateServiceRecipients);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesSetOrder(
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>());

            serviceRecipientService.Setup(s => s.GetOrder(callOffId)).ReturnsAsync(order);
            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);

            await controller.UpdateAsync(callOffId, model);

            serviceRecipientService.Setup(s => s.GetOrder(callOffId));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesOrder(
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>());

            serviceRecipientService.Setup(s => s.GetOrder(callOffId)).ReturnsAsync(order);
            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);
            serviceRecipientService.Setup(s => s.SetOrder(order, It.IsAny<List<SelectedServiceRecipient>>())).Callback(() =>
            {
                order.SetSelectedServiceRecipients(model.ServiceRecipients
                    .Select(s => new SelectedServiceRecipient { Recipient = new ServiceRecipient(s.OdsCode, s.Name) }).ToList());
            });

            await controller.UpdateAsync(callOffId, model);

            order.SelectedServiceRecipients.Select(r => r.Recipient).Should().BeEquivalentTo(
                model.ServiceRecipients,
                o => o.ComparingByMembers<ServiceRecipientModel>());
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_ReturnsNoContent(
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>());

            serviceRecipientService.Setup(s => s.GetOrder(callOffId)).ReturnsAsync(order);
            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);

            var result = await controller.UpdateAsync(callOffId, model);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
