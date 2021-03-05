using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderItemRecipientExtensionsTests
    {
        [Test]
        [CommonAutoData]
        public static void ToModelList_ReturnsExpectedResult(IReadOnlyList<OrderItemRecipient> recipients)
        {
            var expected = recipients.Select(r => new
            {
                r.DeliveryDate,
                r.Recipient.Name,
                r.Recipient.OdsCode,
                r.Quantity,
            });

            var actual = recipients.ToModelList();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
