using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class DefaultDeliveryDateEntityTypeConfiguration : IEntityTypeConfiguration<DefaultDeliveryDate>
    {
        public void Configure(EntityTypeBuilder<DefaultDeliveryDate> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasKey(d => new { d.OrderId, d.CatalogueItemId, d.PriceId });
            builder.Property(d => d.OrderId).HasMaxLength(10);
            builder.Property(d => d.CatalogueItemId).HasMaxLength(14);
            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_DefaultDeliveryDate_OrderId");
        }
    }
}
