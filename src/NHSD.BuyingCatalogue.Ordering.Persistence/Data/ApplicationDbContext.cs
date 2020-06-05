using System;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public DbSet<Order> Order { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipient { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.Entity<ServiceRecipient>(x => x.HasKey(y => y.OdsCode));
        }
    }
}
