namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public sealed class ServiceRecipientEntity : EntityBase
    {
        public string OdsCode { get; set; }
        public string Name { get; set; }
        public string OrderId { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.ServiceRecipient
            (
                OdsCode,
                Name,
                OrderId
            )
            VALUES
            (
                @OdsCode,
                @Name,
                @OrderId
            );";
    }
}
