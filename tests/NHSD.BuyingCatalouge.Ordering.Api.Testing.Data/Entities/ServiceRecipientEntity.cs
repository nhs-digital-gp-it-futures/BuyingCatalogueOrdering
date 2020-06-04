using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
                @OderId
            );";

        public static async Task<IEnumerable<ServiceRecipientEntity>> FetchServiceRecipientsByOrderId(
            string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<IEnumerable<ServiceRecipientEntity>>(connectionString, @"SELECT
                          OdsCode,
                          Name,
                          OrderId
                          FROM dbo.ServiceRecipient
                          WHERE OrderId = @orderId;", new {orderId}));
        }
    }
}
