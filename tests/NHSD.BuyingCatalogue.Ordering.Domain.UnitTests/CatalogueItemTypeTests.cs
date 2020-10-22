using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CatalogueItemTypeTests
    {
        [Test]
        public static void List_ReturnsExpectedList()
        {
            var actual = CatalogueItemType.List();

            var expected = new List<CatalogueItemType>
            {
                CatalogueItemType.Solution,
                CatalogueItemType.AdditionalService,
                CatalogueItemType.AssociatedService
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void FromId_CatalogueItemTypeId_ReturnsExpectedType()
        {
            var actual = CatalogueItemType.FromId(1);

            actual.Should().Be(CatalogueItemType.Solution);
        }

        [Test]
        public static void FromId_UnknownCatalogueItemTypeId_ReturnsNull()
        {
            var actual = CatalogueItemType.FromId(10);
            actual.Should().BeNull();
        }

        [Test]
        public static void FromName_NullName_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => CatalogueItemType.FromName(null));
        }

        [Test]
        public static void FromName_InvalidCatalogueType_ReturnsNull()
        {
            var type = CatalogueItemType.FromName("DoesNotExist");

            type.Should().BeNull();
        }

        [Test]
        public static void FromName_ReturnsExpectedCatalogueType()
        {
            var type = CatalogueItemType.FromName(nameof(CatalogueItemType.Solution));

            type.Should().Be(CatalogueItemType.Solution);
        }

        [Test]
        public static void GetHashCode_ReturnsDifferentValueForEachSolutionType()
        {
            var additionalServiceHash = CatalogueItemType.AdditionalService.GetHashCode();
            var associatedServiceHash = CatalogueItemType.AssociatedService.GetHashCode();
            var solutionHash = CatalogueItemType.Solution.GetHashCode();

            additionalServiceHash.Should().NotBe(associatedServiceHash);
            additionalServiceHash.Should().NotBe(solutionHash);
            associatedServiceHash.Should().NotBe(solutionHash);
        }

        [TestCase(nameof(ProvisioningType.OnDemand), null, null)]
        [TestCase(nameof(ProvisioningType.OnDemand), TimeUnit.PerMonth, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.OnDemand), TimeUnit.PerYear, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.Patient), null, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.Patient), TimeUnit.PerMonth, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.Patient), TimeUnit.PerYear, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.Declarative), null, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.Declarative), TimeUnit.PerMonth, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.Declarative), TimeUnit.PerYear, TimeUnit.PerYear)]
        public static void InferEstimationPeriod_SolutionCatalogueItemType_ReturnsExpectedEstimationPeriod(
            string provisioningTypeNameInput,
            TimeUnit? estimationPeriodNameInput,
            TimeUnit? expectedEstimationPeriodNameInput)
        {
            var estimationPeriod = estimationPeriodNameInput;
            var provisioningType = Enum.Parse<ProvisioningType>(provisioningTypeNameInput);

            var actual = CatalogueItemType.Solution.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(expectedEstimationPeriodNameInput);
        }

        [TestCase(nameof(ProvisioningType.OnDemand), null, null)]
        [TestCase(nameof(ProvisioningType.OnDemand), TimeUnit.PerMonth, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.OnDemand), TimeUnit.PerYear, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.Patient), null, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.Patient), TimeUnit.PerMonth, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.Patient), TimeUnit.PerYear, TimeUnit.PerMonth)]
        [TestCase(nameof(ProvisioningType.Declarative), null, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.Declarative), TimeUnit.PerMonth, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.Declarative), TimeUnit.PerYear, TimeUnit.PerYear)]
        public static void InferEstimationPeriod_AdditionalServiceCatalogueItemType_ReturnsExpectedEstimationPeriod(
            string provisioningTypeNameInput,
            TimeUnit? estimationPeriodNameInput,
            TimeUnit? expectedEstimationPeriodNameInput)
        {
            var estimationPeriod = estimationPeriodNameInput;
            var provisioningType = Enum.Parse<ProvisioningType>(provisioningTypeNameInput);

            var actual = CatalogueItemType.AdditionalService.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(expectedEstimationPeriodNameInput);
        }

        [TestCase(nameof(ProvisioningType.OnDemand), null, null)]
        [TestCase(nameof(ProvisioningType.OnDemand), TimeUnit.PerYear, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.OnDemand), TimeUnit.PerYear, TimeUnit.PerYear)]
        [TestCase(nameof(ProvisioningType.Declarative), null, null)]
        [TestCase(nameof(ProvisioningType.Declarative), TimeUnit.PerYear, null)]
        [TestCase(nameof(ProvisioningType.Declarative), TimeUnit.PerYear, null)]
        public static void InferEstimationPeriod_AssociatedServiceCatalogueItemType_ReturnsExpectedEstimationPeriod(
            string provisioningTypeNameInput,
            TimeUnit? estimationPeriodNameInput,
            TimeUnit? expectedEstimationPeriodNameInput)
        {
            var estimationPeriod = estimationPeriodNameInput;
            var provisioningType = Enum.Parse<ProvisioningType>(provisioningTypeNameInput);

            var actual = CatalogueItemType.AssociatedService.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(expectedEstimationPeriodNameInput);
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public static void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            CatalogueItemType.Solution.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public static void Equals_Same_AreEqual()
        {
            var instance = CatalogueItemType.Solution;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public static void Equals_Different_AreNotEqual()
        {
            var instance = CatalogueItemType.Solution;
            var comparisonObject = CatalogueItemType.AdditionalService;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
