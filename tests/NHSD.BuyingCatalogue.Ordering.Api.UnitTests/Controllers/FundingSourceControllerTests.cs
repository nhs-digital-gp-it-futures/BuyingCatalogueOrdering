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
    internal static class FundingSourceControllerTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingSourceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNull(
            FundingSourceController controller)
        {
            var result = await controller.GetAsync(default);

            result.Result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderExists_FundingSourceDetailsReturned(
            [Frozen] Mock<IFundingSourceService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            FundingSourceController controller)
        {
            var expected = new GetFundingSourceModel { OnlyGms = true };
            service.Setup(o => o.GetFundingSource(order.CallOffId)).ReturnsAsync(order.FundingSourceOnlyGms);

            var actual = await controller.GetAsync(callOffId);
            service.Verify(o => o.GetFundingSource(order.CallOffId));

            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void PutFundingSourceAsync_NullModel_ThrowsException(
            Order order,
            FundingSourceController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.PutFundingSourceAsync(order, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task PutFundingSourceAsync_OrderIsNull_ReturnsNotFound(
            UpdateFundingSourceModel model,
            FundingSourceController controller)
        {
            var response = await controller.PutFundingSourceAsync(null, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task PutFundingSourceAsync_UpdatesFundingSourceOnlyGms(
            [Frozen] Mock<IFundingSourceService> service,
            Order order,
            UpdateFundingSourceModel model,
            FundingSourceController controller)
        {
            service.Setup(o => o.SetFundingSource(order, model.OnlyGms)).Callback(() =>
            {
                order.FundingSourceOnlyGms = model.OnlyGms;
            });

            await controller.PutFundingSourceAsync(order, model);
            service.Verify(o => o.SetFundingSource(order, model.OnlyGms));

            order.FundingSourceOnlyGms.Should().Be(model.OnlyGms);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task PutFundingSourceAsync_SuccessfulUpdate_ReturnsNoContentResult(
            [Frozen] Mock<IFundingSourceService> service,
            Order order,
            UpdateFundingSourceModel model,
            FundingSourceController controller)
        {
            service.Setup(o => o.SetFundingSource(order, model.OnlyGms)).Callback(() =>
            {
                order.FundingSourceOnlyGms = model.OnlyGms;
            });

            var result = await controller.PutFundingSourceAsync(order, model);
            service.Verify(o => o.SetFundingSource(order, model.OnlyGms));

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
