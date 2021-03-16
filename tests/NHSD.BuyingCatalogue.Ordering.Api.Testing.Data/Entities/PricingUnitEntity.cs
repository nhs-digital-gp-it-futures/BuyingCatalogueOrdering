namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class PricingUnitEntity : EntityBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.PricingUnit
            (
                [Name],
                [Description]
            )
            VALUES
            (
                @Name,
                @Description
            );";
    }
}
