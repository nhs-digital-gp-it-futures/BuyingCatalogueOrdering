using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    internal sealed class CatalogueItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItem>
    {
        public void Configure(EntityTypeBuilder<CatalogueItem> builder)
        {
            builder.ToTable("CatalogueItem");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .IsRequired()
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(256);

            const string catalogueItemTypeId = "CatalogueItemTypeId";

            builder
                .Property(i => i.CatalogueItemType)
                .HasConversion<int>()
                .HasColumnName(catalogueItemTypeId);

            builder.HasOne<CatalogueItem>()
                .WithMany()
                .HasForeignKey(i => i.ParentCatalogueItemId)
                .HasConstraintName("FK_CatalogueItem_ParentCatalogueItem");
        }
    }
}
