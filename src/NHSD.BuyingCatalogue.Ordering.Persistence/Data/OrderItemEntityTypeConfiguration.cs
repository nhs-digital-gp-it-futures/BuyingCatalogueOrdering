using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            const string catalogueItemId = "CatalogueItemId";
            const string orderId = "OrderId";

            builder.ToTable("OrderItem");
            builder.Property<CatalogueItemId>(catalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property<int>(orderId).IsRequired();
            builder.HasKey(orderId, catalogueItemId);

            builder.Property(i => i.Created).HasDefaultValueSql("(GetUtcDate())");
            builder.Property(i => i.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(i => i.DefaultDeliveryDate).HasColumnType("date");
            builder.Property(i => i.LastUpdated).HasDefaultValueSql("(GetUtcDate())");
            builder.Property(i => i.Price).HasColumnType("decimal(18, 4)");

            builder.HasOne(i => i.CatalogueItem)
                .WithMany()
                .HasForeignKey(catalogueItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_CatalogueItem");

            builder
                .Property(i => i.CataloguePriceType)
                .HasConversion<int>()
                .HasColumnName("CataloguePriceTypeId");

            builder
                .Property(i => i.EstimationPeriod)
                .HasConversion<int>()
                .HasColumnName("EstimationPeriodId");

            builder.HasOne<Order>()
                .WithMany(o => o.OrderItems)
                .HasForeignKey(orderId)
                .HasConstraintName("FK_OrderItem_Order");

            builder.HasOne(i => i.PricingUnit)
                .WithMany()
                .HasForeignKey("PricingUnitName")
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_PricingUnit");

            builder
                .Property(i => i.ProvisioningType)
                .HasConversion<int>()
                .HasColumnName("ProvisioningTypeId");

            builder
                .Property(i => i.PriceTimeUnit)
                .HasConversion<int>()
                .HasColumnName("TimeUnitId");
        }
    }
}
