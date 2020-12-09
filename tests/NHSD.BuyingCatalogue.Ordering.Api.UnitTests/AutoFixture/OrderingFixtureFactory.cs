using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal static class OrderingFixtureFactory
    {
        private static readonly ICustomization[] BasicCustomizations =
        {
            new AutoMoqCustomization(),
            new ControllerBaseCustomization(),
            new OrderingCustomization(),
            new EnumValueByNameCustomization(),
        };

        internal static IFixture Create() => new Fixture().Customize(CompositeCustomization());

        internal static IFixture Create(Guid userId, string userName) => new Fixture().Customize(
            CompositeCustomization(new HttpContextAccessorCustomization(userId, userName)));

        private static ICustomization CompositeCustomization(params ICustomization[] additionalCustomizations)
        {
            var customizations = new List<ICustomization>(BasicCustomizations);
            customizations.AddRange(additionalCustomizations);

            return new CompositeCustomization(customizations);
        }
    }
}
