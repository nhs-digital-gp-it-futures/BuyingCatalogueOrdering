using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemSolutionRequestTests
    {
        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_DeliveryDate(
            [Frozen] DateTime deliveryDate,
            CreateOrderItemSolutionRequest request)
        {
            request.DeliveryDate.Should().Be(deliveryDate);
        }
    }
}
