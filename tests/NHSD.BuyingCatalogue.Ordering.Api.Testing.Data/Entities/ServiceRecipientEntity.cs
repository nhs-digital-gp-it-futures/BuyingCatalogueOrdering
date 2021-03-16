namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class ServiceRecipientEntity : EntityBase
    {
        public string OdsCode { get; set; }

        public string Name { get; set; }

        public int OrderId { get; set; }

        protected override string InsertSql =>
            @"INSERT INTO dbo.ServiceRecipient
            (
                OdsCode,
                [Name]
            )
            VALUES
            (
                @OdsCode,
                @Name
            );";
    }
}
