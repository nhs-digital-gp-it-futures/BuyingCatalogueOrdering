using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderItemRecipientEntity : EntityBase
    {
        public int OrderId { get; set; }

        public string CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime DeliveryDate { get; set; } = DateTime.UtcNow.Date;

        protected override string InsertSql => @"
            INSERT INTO dbo.OrderItemRecipients
            (
                OrderId,
                CatalogueItemId,
                OdsCode,
                Quantity,
                DeliveryDate
            )
            VALUES
            (
                @OrderId,
                @CatalogueItemId,
                @OdsCode,
                @Quantity,
                @DeliveryDate
            );";

        public static async Task<IEnumerable<OrderItemRecipientEntity>> FetchByOrderId(
            string connectionString,
            int orderId)
        {
            const string sql = @"
                SELECT OrderId,
                       CatalogueItemId,
                       OdsCode,
                       Quantity,
                       DeliveryDate
                  FROM dbo.OrderItemRecipients
                 WHERE OrderId = @orderId;";

            return await SqlRunner.QueryAsync<OrderItemRecipientEntity>(connectionString, sql, new { orderId });
        }
    }
}
