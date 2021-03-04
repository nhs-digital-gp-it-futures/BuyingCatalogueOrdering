using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CatalogueItemTypeTests
    {
        [TestCase(CatalogueItemType.AdditionalService, "Additional Service")]
        [TestCase(CatalogueItemType.AssociatedService, "Associated Service")]
        [TestCase(CatalogueItemType.Solution, "Catalogue Solution")]
        public static void DisplayName_ReturnsExpectedName(CatalogueItemType itemType, string expectedName)
        {
            var displayName = itemType.DisplayName();

            displayName.Should().Be(expectedName);
        }

        [Test]
        public static void MarkOrderSectionAsViewed_NullOrder_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => CatalogueItemType.AdditionalService.MarkOrderSectionAsViewed(null));
        }

        [Test]
        public static void MarkOrderSectionAsViewed_InvalidType_ThrowsException()
        {
            var order = OrderBuilder.Create().Build();

            Assert.Throws<ArgumentException>(() => CatalogueItemTypeExtensions.MarkOrderSectionAsViewed(OrderingEnums.UndefinedTimeUnit, order));
        }

        [TestCaseSource(nameof(MarkOrderSectionAsViewedTestCases))]
        public static void MarkOrderSectionAsViewed_MarksExpectedSection(
            CatalogueItemType itemType,
            Func<Order, bool> sectionViewed)
        {
            var order = OrderBuilder.Create().Build();
            sectionViewed(order).Should().BeFalse();

            itemType.MarkOrderSectionAsViewed(order);

            sectionViewed(order).Should().BeTrue();
        }

        [TestCaseSource(nameof(InferEstimationPeriodTestCases))]
        public static void InferEstimationPeriod_ReturnsExpectedEstimationPeriod(
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType,
            TimeUnit? estimationPeriod,
            TimeUnit? expectedEstimationPeriod)
        {
            var actual = catalogueItemType.InferEstimationPeriod(provisioningType, estimationPeriod);

            actual.Should().Be(expectedEstimationPeriod);
        }

        private static IEnumerable<ITestCaseData> MarkOrderSectionAsViewedTestCases()
        {
            yield return new TestCaseData(CatalogueItemType.AdditionalService, new Func<Order, bool>(o => o.Progress.AdditionalServicesViewed));
            yield return new TestCaseData(CatalogueItemType.AssociatedService, new Func<Order, bool>(o => o.Progress.AssociatedServicesViewed));
            yield return new TestCaseData(CatalogueItemType.Solution, new Func<Order, bool>(o => o.Progress.CatalogueSolutionsViewed));
        }

        private static IEnumerable<ITestCaseData> InferEstimationPeriodTestCases()
        {
            foreach (var data in AdditionalServiceSolutionData(CatalogueItemType.AdditionalService))
                yield return new TestCaseData(data);

            foreach (var data in AdditionalServiceSolutionData(CatalogueItemType.Solution))
                yield return new TestCaseData(data);

            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, null, null);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, TimeUnit.PerYear, TimeUnit.PerYear);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.OnDemand, TimeUnit.PerMonth, TimeUnit.PerMonth);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, null, null);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, TimeUnit.PerYear, null);
            yield return new TestCaseData(CatalogueItemType.AssociatedService, ProvisioningType.Declarative, TimeUnit.PerMonth, null);
        }

        private static IEnumerable<object[]> AdditionalServiceSolutionData(CatalogueItemType itemType)
        {
            yield return new object[] { itemType, ProvisioningType.OnDemand, null, null };
            yield return new object[] { itemType, ProvisioningType.OnDemand, TimeUnit.PerMonth, TimeUnit.PerMonth };
            yield return new object[] { itemType, ProvisioningType.OnDemand, TimeUnit.PerYear, TimeUnit.PerYear };

            yield return new object[] { itemType, ProvisioningType.Patient, null, TimeUnit.PerMonth };
            yield return new object[] { itemType, ProvisioningType.Patient, TimeUnit.PerMonth, TimeUnit.PerMonth };
            yield return new object[] { itemType, ProvisioningType.Patient, TimeUnit.PerYear, TimeUnit.PerMonth };

            yield return new object[] { itemType, ProvisioningType.Declarative, null, TimeUnit.PerYear };
            yield return new object[] { itemType, ProvisioningType.Declarative, TimeUnit.PerMonth, TimeUnit.PerYear };
            yield return new object[] { itemType, ProvisioningType.Declarative, TimeUnit.PerYear, TimeUnit.PerYear };
        }
    }
}
