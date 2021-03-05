using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class ContactEntityTypeConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contact");
            builder.Property(c => c.FirstName).HasMaxLength(100);
            builder.Property(c => c.LastName).HasMaxLength(100);
            builder.Property(c => c.Email).HasMaxLength(256);
            builder.Property(c => c.Phone).HasMaxLength(35);
        }
    }
}
