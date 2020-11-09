using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemAdditionalServiceRequestTests
    {
        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_CatalogueSolutionId(
            [Frozen] CreateOrderItemModel model,
            CreateOrderItemAdditionalServiceRequest request)
        {
            request.CatalogueSolutionId.Should().Be(model.CatalogueSolutionId);
        }
    }
}
