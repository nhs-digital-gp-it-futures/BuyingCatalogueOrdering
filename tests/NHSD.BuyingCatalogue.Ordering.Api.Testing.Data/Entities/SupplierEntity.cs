using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class SupplierEntity : EntityBase
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int AddressId { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.Supplier
            (
                Id,
                [Name],
                AddressId
            )
            VALUES
            (
                @Id,
                @Name,
                @AddressId
            );";

        public static async Task<SupplierEntity> FetchById(string connectionString, string id)
        {
            const string sql = @"
                SELECT Id,
                       [Name],
                       AddressId
                  FROM dbo.Supplier
                 WHERE Id = @id;";

            return await SqlRunner.QueryFirstAsync<SupplierEntity>(connectionString, sql, new { id });
        }
    }
}
