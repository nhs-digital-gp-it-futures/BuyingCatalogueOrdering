﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class SectionStatusControllerTests
    {
        [Test]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SectionStatusController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_SectionStatusIsNull_ThrowsArgumentNullException))]
        public static void UpdateStatusAsync_SectionStatusIsNull_ThrowsArgumentNullException(
            Order order,
            string sectionId,
            SectionStatusController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await controller.UpdateStatusAsync(order, sectionId, null));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_SectionIdIsNull_ThrowsArgumentNullException))]
        public static void UpdateStatusAsync_SectionIdIsNull_ThrowsArgumentNullException(
            Order order,
            UpdateOrderSectionModel model,
            SectionStatusController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await controller.UpdateStatusAsync(order, null, model));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_OrderIsNull_ReturnsNotFound))]
        public static async Task UpdateStatusAsync_OrderIsNull_ReturnsNotFound(
            string sectionId,
            UpdateOrderSectionModel model,
            SectionStatusController controller)
        {
            var result = await controller.UpdateStatusAsync(null, sectionId, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_WrongSectionId_ReturnsForbidden))]
        public static async Task UpdateStatusAsync_WrongSectionId_ReturnsForbidden(
            Order order,
            string sectionId,
            UpdateOrderSectionModel model,
            SectionStatusController controller)
        {
            var result = await controller.UpdateStatusAsync(order, sectionId, model);

            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        [InMemoryDbInlineAutoData(nameof(UpdateStatusAsync_WithSectionId_UpdatesTheSelectedStatus), "additional-services", true, false, false)]
        [InMemoryDbInlineAutoData(nameof(UpdateStatusAsync_WithSectionId_UpdatesTheSelectedStatus), "catalogue-solutions", false, true, false)]
        [InMemoryDbInlineAutoData(nameof(UpdateStatusAsync_WithSectionId_UpdatesTheSelectedStatus), "associated-services", false, false, true)]
        public static async Task UpdateStatusAsync_WithSectionId_UpdatesTheSelectedStatus(
            string sectionId,
            bool additionalServicesViewed,
            bool catalogueSolutionsViewed,
            bool associatedServicesViewed,
            Order order,
            SectionStatusController controller)
        {
            order.Progress.AdditionalServicesViewed = false;
            order.Progress.AssociatedServicesViewed = false;
            order.Progress.CatalogueSolutionsViewed = false;
            order.Progress.ServiceRecipientsViewed = false;

            await controller.UpdateStatusAsync(order, sectionId, new UpdateOrderSectionModel { Status = "complete" });

            order.Progress.AdditionalServicesViewed.Should().Be(additionalServicesViewed);
            order.Progress.AssociatedServicesViewed.Should().Be(associatedServicesViewed);
            order.Progress.CatalogueSolutionsViewed.Should().Be(catalogueSolutionsViewed);
            order.Progress.ServiceRecipientsViewed.Should().BeFalse();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_WithSectionId_UpdatesDb))]
        public static async Task UpdateStatusAsync_WithSectionId_UpdatesDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            SectionStatusController controller)
        {
            order.Progress.CatalogueSolutionsViewed = false;
            context.Order.Add(order);
            await context.SaveChangesAsync();

            await controller.UpdateStatusAsync(
                order,
                "catalogue-solutions",
                new UpdateOrderSectionModel { Status = "complete" });

            context.Set<Order>().First().Progress.CatalogueSolutionsViewed.Should().BeTrue();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_CorrectSectionId_ReturnsNoContentResult))]
        public static async Task UpdateStatusAsync_CorrectSectionId_ReturnsNoContentResult(
            Order order,
            SectionStatusController controller)
        {
            var response = await controller.UpdateStatusAsync(
                order,
                "catalogue-solutions",
                new UpdateOrderSectionModel { Status = "complete" });

            response.Should().BeOfType<NoContentResult>();
        }
    }
}
