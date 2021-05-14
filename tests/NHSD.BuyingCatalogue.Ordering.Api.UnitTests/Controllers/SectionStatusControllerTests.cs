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
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
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
        [InMemoryDbAutoData]
        public static void UpdateStatusAsync_SectionStatusIsNull_ThrowsArgumentNullException(
            string sectionId,
            SectionStatusController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await controller.UpdateStatusAsync(default, sectionId, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateStatusAsync_SectionIdIsNull_ThrowsArgumentNullException(
            UpdateOrderSectionModel model,
            SectionStatusController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await controller.UpdateStatusAsync(default, null, model));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_WrongSectionId_ReturnsForbidden(
            string sectionId,
            [Frozen] Mock<ISectionStatusService> service,
            Order order,
            UpdateOrderSectionModel model,
            SectionStatusController controller)
        {
            service.Setup(o => o.GetOrder(order.CallOffId)).ReturnsAsync(order);
            var result = await controller.UpdateStatusAsync(order.CallOffId, sectionId, model);

            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_OrderNotFound_ReturnsNotFound(
            [Frozen] Mock<ISectionStatusService> service,
            CallOffId callOffId,
            string sectionId,
            UpdateOrderSectionModel model,
            SectionStatusController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync((Order)null);
            var result = await controller.UpdateStatusAsync(callOffId, sectionId, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbInlineAutoData("additional-services", true, false, false)]
        [InMemoryDbInlineAutoData("catalogue-solutions", false, true, false)]
        [InMemoryDbInlineAutoData("associated-services", false, false, true)]
        public static async Task UpdateStatusAsync_WithSectionId_UpdatesTheSelectedStatus(
            string sectionId,
            bool additionalServicesViewed,
            bool catalogueSolutionsViewed,
            bool associatedServicesViewed,
            [Frozen] Mock<ISectionStatusService> service,
            Order order,
            SectionStatusController controller)
        {
            order.Progress.AdditionalServicesViewed = false;
            order.Progress.AssociatedServicesViewed = false;
            order.Progress.CatalogueSolutionsViewed = false;

            service.Setup(o => o.GetOrder(order.CallOffId)).ReturnsAsync(order);
            service.Setup(o => o.SetSectionStatus(order, It.IsAny<string>())).Callback(() =>
            {
                order.Progress.CatalogueSolutionsViewed = catalogueSolutionsViewed;
                order.Progress.AdditionalServicesViewed = additionalServicesViewed;
                order.Progress.AssociatedServicesViewed = associatedServicesViewed;
            });

            await controller.UpdateStatusAsync(order.CallOffId, sectionId, new UpdateOrderSectionModel { Status = "complete" });

            order.Progress.AdditionalServicesViewed.Should().Be(additionalServicesViewed);
            order.Progress.AssociatedServicesViewed.Should().Be(associatedServicesViewed);
            order.Progress.CatalogueSolutionsViewed.Should().Be(catalogueSolutionsViewed);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_CorrectSectionId_ReturnsNoContentResult(
            [Frozen] Mock<ISectionStatusService> service,
            Order order,
            SectionStatusController controller)
        {
            service.Setup(o => o.GetOrder(order.CallOffId)).ReturnsAsync(order);
            service.Setup(o => o.SetSectionStatus(order, It.IsAny<string>())).Callback(() =>
            {
                order.Progress.CatalogueSolutionsViewed = true;
            });

            var response = await controller.UpdateStatusAsync(
                order.CallOffId,
                "catalogue-solutions",
                new UpdateOrderSectionModel { Status = "complete" });

            response.Should().BeOfType<NoContentResult>();
        }
    }
}
