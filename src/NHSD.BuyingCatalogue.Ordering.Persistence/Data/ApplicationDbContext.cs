using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public static readonly ILoggerFactory DbLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipient { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder is null)
                throw new ArgumentNullException(nameof(optionsBuilder));

            base.OnConfiguring(optionsBuilder);

            optionsBuilder
                .UseLoggerFactory(DbLoggerFactory)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceRecipientConfiguration());
        }
    }
}
