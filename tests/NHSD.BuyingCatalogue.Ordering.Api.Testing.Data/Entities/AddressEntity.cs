using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class AddressEntity : EntityBase
    {
        public int Id { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Line3 { get; set; }

        public string Line4 { get; set; }

        public string Line5 { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        protected override string InsertSql => @"
            SET IDENTITY_INSERT dbo.[Address] ON;

            INSERT INTO dbo.[Address]
            (
                Id,
                Line1,
                Line2,
                Line3,
                Line4,
                Line5,
                Town,
                County,
                Postcode,
                Country
            )
            VALUES
            (
                @Id,
                @Line1,
                @Line2,
                @Line3,
                @Line4,
                @Line5,
                @Town,
                @County,
                @Postcode,
                @Country
            );

            SET IDENTITY_INSERT dbo.[Address] OFF;";

        public static async Task<AddressEntity> FetchAddressById(string connectionString, int? id)
        {
            if (id is null)
                return null;

            const string sql = @"
                SELECT Id,
                       Line1,
                       Line2,
                       Line3,
                       Line4,
                       Line5,
                       Town,
                       County,
                       Postcode,
                       Country
                  FROM dbo.[Address]
                 WHERE Id = @id;";

            return await SqlRunner.QueryFirstAsync<AddressEntity>(connectionString, sql, new { id });
        }
    }
}
