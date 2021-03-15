using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ServiceInstanceItemEntityTypeConfiguration : IEntityTypeConfiguration<ServiceInstanceItem>
    {
        public void Configure(EntityTypeBuilder<ServiceInstanceItem> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToView(nameof(ServiceInstanceItem));
            builder.HasKey(i => new { i.OrderId, i.CatalogueItemId, i.OdsCode });
        }
    }
}
