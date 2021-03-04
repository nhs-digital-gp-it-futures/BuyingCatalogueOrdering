using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class OrderProgressEntityTypeConfiguration : IEntityTypeConfiguration<OrderProgress>
    {
        public void Configure(EntityTypeBuilder<OrderProgress> builder)
        {
            builder.ToTable("OrderProgress");
            builder.Property(o => o.OrderId).ValueGeneratedNever();
            builder.HasKey(o => o.OrderId);
            builder.HasOne<Order>()
                .WithOne(o => o.Progress)
                .HasForeignKey<OrderProgress>(o => o.OrderId)
                .HasConstraintName("FK_OrderProgress_Order");
        }
    }
}
