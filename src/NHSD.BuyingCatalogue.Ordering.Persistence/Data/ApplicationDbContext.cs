using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        private readonly IIdentityService identityService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IIdentityService identityService)
            : base(options)
        {
            this.identityService = identityService;
        }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderingParty> OrderingParty { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipient { get; set; }

        public DbSet<DefaultDeliveryDate> DefaultDeliveryDate { get; set; }

        public DbSet<Supplier> Supplier { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is not IAudited auditedEntity)
                    continue;

                (Guid userId, string userName) = identityService.GetUserInfo();

                switch (entry.State)
                {
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        continue;
                    default:
                        auditedEntity.SetLastUpdatedBy(userId, userName);
                        continue;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AddressEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogueItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContactEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultDeliveryDateEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderingPartyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemRecipientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProgressEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PricingUnitEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceInstanceItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SelectedServiceRecipientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceRecipientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierEntityTypeConfiguration());
        }
    }
}
