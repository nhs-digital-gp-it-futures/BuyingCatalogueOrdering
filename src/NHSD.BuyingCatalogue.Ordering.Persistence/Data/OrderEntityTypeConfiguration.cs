using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasConversion(
                    input => input.Value,
                    output => OrderDescription.Create(output).Value)
                .IsRequired();

            builder.Property(o => o.OrganisationId)
                .HasColumnName("OrganisationId")
                .IsRequired();

            builder.Property(x => x.OrderStatus)
                .HasColumnName("OrderStatusId")
                .HasConversion<int>();

            builder.HasMany(x => x.OrderItems)
                .WithOne();

            builder.HasMany(x => x.ServiceRecipients)
                .WithOne(x => x.Order);
        }
    }
}
