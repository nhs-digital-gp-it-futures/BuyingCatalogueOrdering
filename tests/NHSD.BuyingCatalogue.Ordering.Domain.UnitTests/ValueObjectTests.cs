using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    public sealed class ValueObjectTests
    {
        private Size size1 = new(1, 2);

        [Test]
        public void GivenSameValuesShouldReturnTrue()
        {
            var size2 = new Size(1, 2);

            size1.Equals(size2).Should().BeTrue();
            (size1 == size2).Should().BeTrue();
        }

        [Test]
        public void GivenDifferentValuesShouldReturnFalse()
        {
            var size2 = new Size(2, 1);

            size1.Equals(size2).Should().BeFalse();
            (size1 != size2).Should().BeTrue();
        }

        [Test]
        public void GivenNullComparisonShouldReturnFalse()
        {
            size1.Equals(null).Should().BeFalse();
            (size1 == null).Should().BeFalse();
        }

        [Test]
        [SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "Test of null equality")]
        public void GivenTwoNullsShouldReturnTrue()
        {
            size1 = null;
            (size1 == null).Should().BeTrue();
        }

        [Test]
        public void GivenInvalidObjectTypeShouldReturnFalse()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            size1.Equals("Yo Heave Ho").Should().BeFalse();
        }

        private sealed class Size : ValueObject
        {
            public Size(int width, int height)
            {
                Width = width;
                Height = height;
            }

            private int Width { get; }

            private int Height { get; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return Width;
                yield return Height;
            }
        }
    }
}
