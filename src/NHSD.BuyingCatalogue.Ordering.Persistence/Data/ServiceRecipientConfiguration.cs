using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ServiceRecipientConfiguration : IEntityTypeConfiguration<ServiceRecipient>
    {
        public void Configure(EntityTypeBuilder<ServiceRecipient> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasKey(x => x.OdsCode);
        }
    }
}
