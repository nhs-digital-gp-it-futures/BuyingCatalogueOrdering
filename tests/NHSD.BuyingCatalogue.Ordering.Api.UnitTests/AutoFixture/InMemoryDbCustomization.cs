using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class InMemoryDbCustomization : ICustomization
    {
        private readonly DbContextOptions<ApplicationDbContext> dbContextOptions;

        public InMemoryDbCustomization(string dbName)
        {
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(dbName)
              .Options;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<ApplicationDbContext>(_ => new MockIdentityServiceSpecimenBuilder());
            fixture.Customize<ApplicationDbContext>(_ => new ApplicationDbContextSpecimenBuilder(dbContextOptions));
        }

        private sealed class ApplicationDbContextSpecimenBuilder : ISpecimenBuilder
        {
            private readonly DbContextOptions<ApplicationDbContext> dbContextOptions;

            internal ApplicationDbContextSpecimenBuilder(DbContextOptions<ApplicationDbContext> dbContextOptions) =>
                this.dbContextOptions = dbContextOptions;

            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(ApplicationDbContext)))
                    return new NoSpecimen();

                var identityService = context.Create<IIdentityService>();

                var dbContext = new ApplicationDbContext(dbContextOptions, identityService);
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                return dbContext;
            }
        }
    }
}
