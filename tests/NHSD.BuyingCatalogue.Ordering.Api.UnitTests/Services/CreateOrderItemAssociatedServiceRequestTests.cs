using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemAssociatedServiceRequestTests
    {
        [Test]
        [OrderingAutoData]
        public static void Constructor_Initializes_OdsCode(
            [Frozen] Order order,
            CreateOrderItemAssociatedServiceRequest request)
        {
            request.OdsCode.Should().Be(order.OrganisationOdsCode);
        }
    }
}
