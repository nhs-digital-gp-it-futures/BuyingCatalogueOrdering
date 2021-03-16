using System;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderingPartyEntity : EntityBase
    {
        public Guid Id { get; init; }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public int? AddressId { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.OrderingParty
            (
                Id,
                OdsCode,
                [Name],
                AddressId
            )
            VALUES
            (
                @Id,
                @OdsCode,
                @Name,
                @AddressId
            );";

        public static async Task<OrderingPartyEntity> FetchById(string connectionString, Guid? id)
        {
            if (id is null)
                return null;

            const string sql = @"
                SELECT Id,
                       OdsCode,
                       [Name],
                       AddressId
                  FROM dbo.OrderingParty
                 WHERE Id = @id;";

            return await SqlRunner.QueryFirstAsync<OrderingPartyEntity>(connectionString, sql, new { id });
        }
    }
}
