using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Domain.Orders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderDescriptionTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Create_DescriptionIsNullOrWhitespace_ReturnsFailureResult(string description)
        {
            var isValid = OrderDescription.Create(description);

            isValid.IsSuccess.Should().BeFalse();
            isValid.Errors.Should().BeEquivalentTo(OrderErrors.OrderDescriptionRequired());
        }

        [Test]
        public static void Create_DescriptionExceedsMaxLength_ReturnsFailureResult()
        {
            var isValid = OrderDescription.Create(new string('a', 101));

            isValid.IsSuccess.Should().BeFalse();
            isValid.Errors.Should().BeEquivalentTo(OrderErrors.OrderDescriptionTooLong());
        }

        [Test]
        public static void Create_DescriptionIsValid_ReturnsSuccessResult()
        {
            var isValid = OrderDescription.Create("New Description");

            isValid.IsSuccess.Should().BeTrue();
            isValid.Errors.Should().BeEmpty();
        }
    }
}
