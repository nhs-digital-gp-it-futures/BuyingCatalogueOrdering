using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class SelectedServiceRecipientEntity : EntityBase
    {
        public int OrderId { get; set; }

        public string OdsCode { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.SelectedServiceRecipients (OrderId, OdsCode)
            VALUES (@OrderId, @OdsCode);";

        public static async Task<IEnumerable<SelectedServiceRecipientEntity>> FetchAllServiceRecipients(string connectionString)
        {
            const string sql =
                @"SELECT OrderId, OdsCode
                    FROM dbo.SelectedServiceRecipients;";

            return await SqlRunner.QueryAsync<SelectedServiceRecipientEntity>(connectionString, sql);
        }
    }
}
