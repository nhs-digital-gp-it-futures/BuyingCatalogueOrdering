using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class OrderExtensions
    {
        [Test]
        public void IsSupplierSectionComplete_HasPrimaryContact_ReturnsTrue()
        {
            var order = OrderBuilder
                .Create()
                .WithSupplierContact(ContactBuilder.Create().Build())
                .Build();

            order.IsSupplierSectionComplete().Should().BeTrue();
        }

        [Test]
        public void IsSupplierSectionComplete_NullPrimaryContact_ReturnsFalse()
        {
            var order = OrderBuilder
                .Create()
                .WithSupplierContact(null)
                .Build();

            order.IsSupplierSectionComplete().Should().BeFalse();
        }
    }
}
