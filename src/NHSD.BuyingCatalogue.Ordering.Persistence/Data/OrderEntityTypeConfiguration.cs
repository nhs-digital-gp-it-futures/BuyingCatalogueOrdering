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
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Property(o => o.Description)
                .HasColumnName("Description")
                .HasConversion(
                    orderDescription => orderDescription.Value,
                    description => OrderDescription.Create(description).Value)
                .IsRequired();

            builder.Property(o => o.OrganisationId)
                .HasColumnName("OrganisationId")
                .IsRequired();

            builder.HasMany(o => o.OrderItems)
                .WithOne();

            builder.HasMany(o => o.ServiceRecipients)
                .WithOne(r => r.Order);

            builder
                .Property(o => o.OrderStatus)
                .HasConversion(status => status.Id, id => OrderStatus.FromId(id))
                .HasColumnName("OrderStatusId");

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.Property(o => o.Completed)
                .HasCamelCaseBackingField(nameof(Order.Completed))
                .HasColumnName("Completed");
        }
    }
}
