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
        [InMemoryDbAutoData(nameof(GetAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            FundingSourceController controller)
        {
            var result = await controller.GetAsync(callOffId);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAsync_OrderExists_FundingSourceDetailsReturned))]
        public static async Task GetAsync_OrderExists_FundingSourceDetailsReturned(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            FundingSourceController controller)
        {
            order.FundingSourceOnlyGms = true;
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expected = new GetFundingSourceModel { OnlyGms = true };

            var actual = await controller.GetAsync(callOffId);

            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData(nameof(PutFundingSourceAsync_NullModel_ThrowsException))]
        public static void PutFundingSourceAsync_NullModel_ThrowsException(
            Order order,
            FundingSourceController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.PutFundingSourceAsync(order, null));
        }

        [Test]
        [InMemoryDbAutoData(nameof(PutFundingSourceAsync_OrderIsNull_ReturnsNotFound))]
        public static async Task PutFundingSourceAsync_OrderIsNull_ReturnsNotFound(
            UpdateFundingSourceModel model,
            FundingSourceController controller)
        {
            var response = await controller.PutFundingSourceAsync(null, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(PutFundingSourceAsync_UpdatesFundingSourceOnlyGms))]
        public static async Task PutFundingSourceAsync_UpdatesFundingSourceOnlyGms(
            Order order,
            UpdateFundingSourceModel model,
            FundingSourceController controller)
        {
            order.FundingSourceOnlyGms = false;
            model.OnlyGms = true;

            await controller.PutFundingSourceAsync(order, model);

            order.FundingSourceOnlyGms.Should().BeTrue();
        }

        [Test]
        [InMemoryDbAutoData(nameof(PutFundingSourceAsync_SuccessfulUpdate_ReturnsNoContentResult))]
        public static async Task PutFundingSourceAsync_SuccessfulUpdate_ReturnsNoContentResult(
            Order order,
            UpdateFundingSourceModel model,
            FundingSourceController controller)
        {
            var result = await controller.PutFundingSourceAsync(order, model);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(PutFundingSourceAsync_SavesChangesToDb))]
        public static async Task PutFundingSourceAsync_SavesChangesToDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            UpdateFundingSourceModel model,
            FundingSourceController controller)
        {
            order.FundingSourceOnlyGms = false;
            model.OnlyGms = true;

            context.Add(order);
            await context.SaveChangesAsync();

            await controller.PutFundingSourceAsync(order, model);

            context.Set<Order>().First(o => o.Equals(order)).FundingSourceOnlyGms.Should().BeTrue();
        }
    }
}
