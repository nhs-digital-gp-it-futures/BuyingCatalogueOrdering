using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.CallOffId)
                .IsRequired()
                .HasMaxLength(4000)
                .HasComputedColumnSql("(concat('C',format([Id],'000000'),'-',format([Revision],'00')))", false)
                .HasConversion(id => id.ToString(), id => CallOffId.Parse(id).Id);

            builder.Property(o => o.CommencementDate).HasColumnType("date");
            builder.Property(o => o.Created).HasDefaultValueSql("(getutcdate())");

            builder.Property(o => o.Description)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.FundingSourceOnlyGms).HasColumnName("FundingSourceOnlyGMS");
            builder.Property(o => o.LastUpdated).HasDefaultValueSql("(getutcdate())");
            builder.Property(o => o.LastUpdatedByName).HasMaxLength(256);
            builder.Property(o => o.Revision).HasDefaultValueSql("((1))");
            builder.Property(o => o.OrderStatus)
                .HasConversion(status => status.Id, id => OrderStatus.FromId(id))
                .HasColumnName("OrderStatusId");

            builder.HasOne(o => o.OrderingPartyContact)
                .WithMany()
                .HasForeignKey("OrderingPartyContactId")
                .HasConstraintName("FK_Order_OrderingPartyContact");

            builder.HasOne(o => o.OrderingParty)
                .WithMany(p => p.Orders)
                .HasForeignKey("OrderingPartyId")
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_OrderingParty")
                .IsRequired();

            builder.HasOne(o => o.SupplierContact)
                .WithMany()
                .HasForeignKey("SupplierContactId")
                .HasConstraintName("FK_Order_SupplierContact");

            builder.HasQueryFilter(o => !o.IsDeleted);
            builder.HasIndex(o => o.IsDeleted, "IX_Order_IsDeleted");
        }
    }
}
