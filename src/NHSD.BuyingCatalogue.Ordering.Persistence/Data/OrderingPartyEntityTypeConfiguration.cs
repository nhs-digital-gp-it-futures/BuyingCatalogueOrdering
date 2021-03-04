using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class OrderingPartyEntityTypeConfiguration : IEntityTypeConfiguration<OrderingParty>
    {
        public void Configure(EntityTypeBuilder<OrderingParty> builder)
        {
            builder.ToTable("OrderingParty");
            builder.HasKey(o => o.Id).IsClustered(false);
            builder.Property(o => o.Id).ValueGeneratedNever();
            builder.Property(o => o.Name).HasMaxLength(256);

            builder.Property(o => o.OdsCode).HasMaxLength(8);
            builder.HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey("AddressId")
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderingParty_Address");

            builder.HasIndex(o => o.OdsCode, "AK_OrderingParty_OdsCode")
                .IsUnique()
                .IsClustered();
        }
    }
}
