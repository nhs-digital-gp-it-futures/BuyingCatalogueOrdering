using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class CatalogueItemEntity : EntityBase
    {
        public string Id { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public string Name { get; set; }

        [UsedImplicitly]
        public string ParentCatalogueItemId { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.CatalogueItem
            (
                Id,
                CatalogueItemTypeId,
                [Name],
                ParentCatalogueItemId
            )
            VALUES
            (
                @Id,
                @CatalogueItemType,
                @Name,
                @ParentCatalogueItemId
            );";
    }
}
