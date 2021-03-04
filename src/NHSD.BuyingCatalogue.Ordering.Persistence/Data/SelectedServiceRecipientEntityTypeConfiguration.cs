using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class SelectedServiceRecipientEntityTypeConfiguration : IEntityTypeConfiguration<SelectedServiceRecipient>
    {
        public void Configure(EntityTypeBuilder<SelectedServiceRecipient> builder)
        {
            const string odsCode = "OdsCode";
            const string orderId = "OrderId";

            builder.ToTable("SelectedServiceRecipients");
            builder.Property<int>(orderId).IsRequired();
            builder.Property<string>(odsCode).HasMaxLength(8).IsRequired();
            builder.HasKey(orderId, odsCode);

            builder.HasOne(r => r.Recipient)
                .WithMany()
                .HasForeignKey(odsCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SelectedServiceRecipients_OdsCode");

            builder.HasOne<Order>()
                .WithMany(o => o.SelectedServiceRecipients)
                .HasForeignKey(orderId)
                .HasConstraintName("FK_SelectedServiceRecipients_Order");
        }
    }
}
