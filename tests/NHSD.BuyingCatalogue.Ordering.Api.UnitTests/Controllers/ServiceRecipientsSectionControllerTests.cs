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
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
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
        [InMemoryDbAutoData(nameof(GetAllAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task GetAllAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            ServiceRecipientsSectionController controller)
        {
            var response = await controller.GetAllAsync(callOffId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAllAsync_NoServiceRecipient_ReturnsEmptyList))]
        public static async Task GetAllAsync_NoServiceRecipient_ReturnsEmptyList(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = new List<ServiceRecipientModel>(),
            };

            var response = await controller.GetAllAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAllAsync_HasSelectedServiceRecipients_ReturnsExpectedResult))]
        public static async Task GetAllAsync_HasSelectedServiceRecipients_ReturnsExpectedResult(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            IReadOnlyList<SelectedServiceRecipient> serviceRecipients,
            Order order,
            ServiceRecipientsSectionController controller)
        {
            order.SetSelectedServiceRecipients(serviceRecipients);
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = serviceRecipients.Select(r => new ServiceRecipientModel(r)).ToList(),
            };

            var response = await controller.GetAllAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_ModelIsNull_ThrowsArgumentNullException))]
        public static void UpdateAsync_ModelIsNull_ThrowsArgumentNullException(
            ServiceRecipientsSectionController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(default, null));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task UpdateAsync_OrderDoesNotExist_ReturnsNotFound(
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            var response = await controller.UpdateAsync(default, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task UpdateAsync_InvokesAddOrUpdateServiceRecipients(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.Is<IEnumerable<ServiceRecipientModel>>(r => r == model.ServiceRecipients));

            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);

            await controller.UpdateAsync(callOffId, model);

            serviceRecipientService.Verify(addOrUpdateServiceRecipients);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_UpdatesOrder))]
        public static async Task UpdateAsync_UpdatesOrder(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.Is<IEnumerable<ServiceRecipientModel>>(r => r == model.ServiceRecipients));

            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);

            await controller.UpdateAsync(callOffId, model);

            order.SelectedServiceRecipients.Select(r => r.Recipient).Should().BeEquivalentTo(
                model.ServiceRecipients,
                o => o.ComparingByMembers<ServiceRecipientModel>());
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_UpdatesDb))]
        public static async Task UpdateAsync_UpdatesDb(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.Is<IEnumerable<ServiceRecipientModel>>(r => r == model.ServiceRecipients));

            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);

            await controller.UpdateAsync(callOffId, model);

            context.Order.Single().SelectedServiceRecipients.Select(r => r.Recipient).Should().BeEquivalentTo(
                model.ServiceRecipients,
                o => o.ComparingByMembers<ServiceRecipientModel>());
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_ReturnsNoContent))]
        public static async Task UpdateAsync_ReturnsNoContent(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            [Frozen] CallOffId callOffId,
            Order order,
            ServiceRecipientsModel model,
            ServiceRecipientsSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var recipients = model.ServiceRecipients
                .Select(r => new ServiceRecipient(r.OdsCode, r.Name))
                .ToDictionary(r => r.OdsCode);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            Expression<Func<IServiceRecipientService, Task<IReadOnlyDictionary<string, ServiceRecipient>>>> addOrUpdateServiceRecipients = s =>
                s.AddOrUpdateServiceRecipients(It.Is<IEnumerable<ServiceRecipientModel>>(r => r == model.ServiceRecipients));

            serviceRecipientService.Setup(addOrUpdateServiceRecipients).ReturnsAsync(recipients);

            var result = await controller.UpdateAsync(callOffId, model);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
