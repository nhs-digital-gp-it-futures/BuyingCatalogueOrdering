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
        private Size _size1 = new Size(1, 2);

        [Test]
        public void GivenSameValuesShouldReturnTrue()
        {
            var size2 = new Size(1, 2);

            _size1.Equals(size2).Should().BeTrue();
            (_size1 == size2).Should().BeTrue();
        }

        [Test]
        public void GivenDifferentValuesShouldReturnFalse()
        {
            var size2 = new Size(2, 1);

            _size1.Equals(size2).Should().BeFalse();
            (_size1 != size2).Should().BeTrue();
        }

        [Test]
        public void GivenNullComparisonShouldReturnFalse()
        {
            ValueObject size2 = null;
            _size1.Equals(size2).Should().BeFalse();
            (_size1 == size2).Should().BeFalse();
            (size2 == _size1).Should().BeFalse();
        }

        [Test]
        [SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "Test of null equality")]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull", Justification = "Test of null equality")]
        public void GivenTwoNullsShouldReturnTrue()
        {
            _size1 = null;
            ValueObject size2 = null;
            (_size1 == size2).Should().BeTrue();
        }

        [Test]
        public void GivenInvalidObjectTypeShouldReturnFalse()
        {
            _size1.Equals("Yo Heave Ho").Should().BeFalse();
        }

        private class Size : ValueObject
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
