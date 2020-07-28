using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CatalogueItemTypeTests
    {
        [Test]
        public void List_ReturnsExpectedList()
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
        public void FromId_CatalogueItemTypeId_ReturnsExpectedType()
        {
            var actual = CatalogueItemType.FromId(1);

            actual.Should().Be(CatalogueItemType.Solution);
        }

        [Test]
        public void FromId_UnknownCatalogueItemTypeId_ReturnsNull()
        {
            var actual = CatalogueItemType.FromId(10);
            actual.Should().BeNull();
        }

        [Test]
        public void InferEstimationPeriod_NullProvisioningType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                CatalogueItemType.Solution.InferEstimationPeriod(null, TimeUnit.PerYear));
        }

        [TestCase(nameof(ProvisioningType.OnDemand), null, null)]
        [TestCase(nameof(ProvisioningType.OnDemand), "month", "month")]
        [TestCase(nameof(ProvisioningType.OnDemand), "year", "year")]
        [TestCase(nameof(ProvisioningType.Patient), null, "month")]
        [TestCase(nameof(ProvisioningType.Patient), "month", "month")]
        [TestCase(nameof(ProvisioningType.Patient), "year", "month")]
        [TestCase(nameof(ProvisioningType.Declarative), null, "year")]
        [TestCase(nameof(ProvisioningType.Declarative), "month", "year")]
        [TestCase(nameof(ProvisioningType.Declarative), "year", "year")]
        public void InferEstimationPeriod_SolutionCatalogueItemType_ReturnsExpectedEstimationPeriod(
            string provisioningTypeNameInput,
            string estimationPeriodNameInput,
            string expectedEstimationPeriodNameInput)
        {
            var estimationPeriod = TimeUnit.FromName(estimationPeriodNameInput);
            var provisioningType = ProvisioningType.FromName(provisioningTypeNameInput);

            var actual = CatalogueItemType.Solution.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(TimeUnit.FromName(expectedEstimationPeriodNameInput));
        }

        [TestCase(nameof(ProvisioningType.OnDemand), null, null)]
        [TestCase(nameof(ProvisioningType.OnDemand), "month", "month")]
        [TestCase(nameof(ProvisioningType.OnDemand), "year", "year")]
        [TestCase(nameof(ProvisioningType.Patient), null, "month")]
        [TestCase(nameof(ProvisioningType.Patient), "month", "month")]
        [TestCase(nameof(ProvisioningType.Patient), "year", "month")]
        [TestCase(nameof(ProvisioningType.Declarative), null, "year")]
        [TestCase(nameof(ProvisioningType.Declarative), "month", "year")]
        [TestCase(nameof(ProvisioningType.Declarative), "year", "year")]
        public void InferEstimationPeriod_AdditionalServiceCatalogueItemType_ReturnsExpectedEstimationPeriod(
            string provisioningTypeNameInput,
            string estimationPeriodNameInput,
            string expectedEstimationPeriodNameInput)
        {
            var estimationPeriod = TimeUnit.FromName(estimationPeriodNameInput);
            var provisioningType = ProvisioningType.FromName(provisioningTypeNameInput);

            var actual = CatalogueItemType.AdditionalService.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(TimeUnit.FromName(expectedEstimationPeriodNameInput));
        }

        [TestCase(nameof(ProvisioningType.OnDemand), null, null)]
        [TestCase(nameof(ProvisioningType.OnDemand), "month", "month")]
        [TestCase(nameof(ProvisioningType.OnDemand), "year", "year")]
        [TestCase(nameof(ProvisioningType.Declarative), null, null)]
        [TestCase(nameof(ProvisioningType.Declarative), "month", null)]
        [TestCase(nameof(ProvisioningType.Declarative), "year", null)]
        public void InferEstimationPeriod_AssociatedServiceCatalogueItemType_ReturnsExpectedEstimationPeriod(
            string provisioningTypeNameInput,
            string estimationPeriodNameInput,
            string expectedEstimationPeriodNameInput)
        {
            var estimationPeriod = TimeUnit.FromName(estimationPeriodNameInput);
            var provisioningType = ProvisioningType.FromName(provisioningTypeNameInput);

            var actual = CatalogueItemType.AssociatedService.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(TimeUnit.FromName(expectedEstimationPeriodNameInput));
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            CatalogueItemType.Solution.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public void Equals_Same_AreEqual()
        {
            var instance = CatalogueItemType.Solution;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public void Equals_Different_AreNotEqual()
        {
            var instance = CatalogueItemType.Solution;
            var comparisonObject = CatalogueItemType.AdditionalService;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
