using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class OrderItemRecipientEntityTypeConfiguration : IEntityTypeConfiguration<OrderItemRecipient>
    {
        public void Configure(EntityTypeBuilder<OrderItemRecipient> builder)
        {
            const string catalogueItemId = "CatalogueItemId";
            const string odsCode = "OdsCode";
            const string orderId = "OrderId";

            builder.ToTable("OrderItemRecipients");
            builder.Property<int>(orderId).IsRequired();
            builder.Property<CatalogueItemId>(catalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property<string>(odsCode).HasMaxLength(8);
            builder.HasKey(orderId, catalogueItemId, odsCode);

            builder.Property(r => r.DeliveryDate).HasColumnType("date");

            builder.HasOne(r => r.Recipient)
                .WithMany()
                .HasForeignKey(odsCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItemRecipients_OdsCode");

            builder.HasOne<OrderItem>()
                .WithMany(i => i.OrderItemRecipients)
                .HasForeignKey(orderId, catalogueItemId)
                .HasConstraintName("FK_OrderItemRecipients_OrderItem");
        }
    }
}
