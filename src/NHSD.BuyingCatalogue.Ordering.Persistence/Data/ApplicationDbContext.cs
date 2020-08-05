using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<Order> Order { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipient { get; set; }

        private readonly IHostEnvironment _hostEnvironment;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHostEnvironment hostEnvironment,
            ILoggerFactory loggerFactory = null)
            : base(options)
        {
            _hostEnvironment = hostEnvironment;
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder?.UseLoggerFactory(_loggerFactory).EnableSensitiveDataLogging(_hostEnvironment.IsDevelopment());
        }

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
        }
    }
}
