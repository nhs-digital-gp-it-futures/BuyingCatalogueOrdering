using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
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

        public static async Task<IEnumerable<ServiceRecipientEntity>> FetchAllServiceRecipients(string connectionString)
        {
            return (await SqlRunner.QueryAsync<ServiceRecipientEntity>(connectionString, @"SELECT
                          OdsCode,
                          Name,
                          OrderId
                         FROM dbo.ServiceRecipient;"));
        }
    }
}
