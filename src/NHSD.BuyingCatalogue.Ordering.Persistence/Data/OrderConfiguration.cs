using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Property(x => x.Description).HasConversion<string>(description => description.Value,
                data => OrderDescription.Create(data).Value);
        }
    }
}
