using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.Property(a => a.Line1).IsRequired().HasMaxLength(256);
            builder.Property(a => a.Line2).HasMaxLength(256);
            builder.Property(a => a.Line3).HasMaxLength(256);
            builder.Property(a => a.Line4).HasMaxLength(256);
            builder.Property(a => a.Line5).HasMaxLength(256);
            builder.Property(a => a.Town).HasMaxLength(256);
            builder.Property(a => a.County).HasMaxLength(256);
            builder.Property(a => a.Postcode).IsRequired().HasMaxLength(10);
            builder.Property(a => a.Country).HasMaxLength(256);
        }
    }
}
