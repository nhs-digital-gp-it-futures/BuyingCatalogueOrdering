using System;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Order { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipient { get; set; }

        public DbSet<DefaultDeliveryDate> DefaultDeliveryDate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceRecipientConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultDeliveryDateEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceInstanceItemEntityTypeConfiguration());
        }
    }
}
