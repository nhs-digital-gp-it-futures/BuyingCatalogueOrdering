using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderProgressEntity : EntityBase
    {
        public int OrderId { get; set; }

        public bool ServiceRecipientsViewed { get; set; }

        public bool CatalogueSolutionsViewed { get; set; }

        public bool AdditionalServicesViewed { get; set; }

        public bool AssociatedServicesViewed { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.OrderProgress
            (
                OrderId,
                ServiceRecipientsViewed,
                CatalogueSolutionsViewed,
                AdditionalServicesViewed,
                AssociatedServicesViewed
            )
            VALUES
            (
                @OrderId,
                @ServiceRecipientsViewed,
                @CatalogueSolutionsViewed,
                @AdditionalServicesViewed,
                @AssociatedServicesViewed
            );";

        public static async Task<OrderProgressEntity> FetchOrderByOrderId(string connectionString, int orderId)
        {
            const string sql = @"
                SELECT OrderId,
                       ServiceRecipientsViewed,
                       CatalogueSolutionsViewed,
                       AdditionalServicesViewed,
                       AssociatedServicesViewed
                  FROM dbo.OrderProgress
                 WHERE OrderId = @orderId;";

            return await SqlRunner.QueryFirstAsync<OrderProgressEntity>(connectionString, sql, new { orderId });
        }
    }
}
