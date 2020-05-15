using System;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrderDescriptionTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Create_DescriptionIsNullOrWhitespace_ReturnsFailureResult(string description)
        {
            var isValid = OrderDescription.Create(description);
        }
    }
}
