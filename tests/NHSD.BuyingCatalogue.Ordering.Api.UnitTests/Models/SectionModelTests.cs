using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class SectionModelTests
    {
        [Test]
        public void Description_ReturnsExpected()
        {
            var actual = SectionModel.Description;

            actual.Id.Should().Be("description");
            actual.Status.Should().Be("complete");
        }

        [Test]
        public void OrderingParty_ReturnsExpected()
        {
            var actual = SectionModel.OrderingParty;

            actual.Id.Should().Be("ordering-party");
            actual.Status.Should().Be("incomplete");
        }

        [Test]
        public void WithStatus_Null_ThrowsException()
        {
            static void Test()
            {
                var actual = SectionModel.OrderingParty;
                actual.WithStatus(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void WithStatus_UnknownStatus_StatusIsEqual()
        {
            const string expected = "unknown";
            var actual = SectionModel.OrderingParty;

            actual.WithStatus(expected);

            actual.Status.Should().Be(expected);
        }

        [Test]
        public void WithStatus_Status_ReturnsSameInstance()
        {
            const string status = "some status";
            var sectionModel = SectionModel.OrderingParty;

            var actual = sectionModel.WithStatus(status);

            actual.Should().Be(sectionModel);
        }

        [Test]
        public void WithCount_Int_CountIsEqual()
        {
            const int expected = 123;
            var actual = SectionModel.OrderingParty;

            actual.WithCount(expected);

            actual.Count.Should().Be(expected);
        }

        [Test]
        public void WithCount_Int_ReturnsSameInstance()
        {
            const int count = 2;
            var sectionModel = SectionModel.OrderingParty;

            var actual = sectionModel.WithCount(count);

            actual.Should().Be(sectionModel);
        }
    }
}
