using System;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture
{
    internal sealed class CatalogueItemIdCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CatalogueItemId>(_ => new CatalogueItemIdSpecimenBuilder());
        }

        private sealed class CatalogueItemIdSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(CatalogueItemId)))
                    return new NoSpecimen();

                var supplierId = (context.Create<int>() % CatalogueItemId.MaxSupplierId) + 1;
                var itemId = (string)context.Resolve(new ConstrainedStringRequest(1, CatalogueItemId.MaxItemIdLength));

                return new CatalogueItemId(supplierId, itemId);
            }
        }
    }
}
