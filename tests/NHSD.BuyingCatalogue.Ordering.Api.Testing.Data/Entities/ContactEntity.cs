using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class ContactEntity : EntityBase
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        protected override string InsertSql => @"
            SET IDENTITY_INSERT dbo.Contact ON;

            INSERT INTO dbo.Contact
            (
                Id,
                FirstName,
                LastName,
                Email,
                Phone
            )
            VALUES
            (
                @Id,
                @FirstName,
                @LastName,
                @Email,
                @Phone
            );

            SET IDENTITY_INSERT dbo.Contact OFF;";

        public static async Task<ContactEntity> FetchContactById(string connectionString, int? id)
        {
            if (id is null)
            {
                return null;
            }

            const string sql =
                @"SELECT Id,
                         FirstName,
                         LastName,
                         Email,
                         Phone
                    FROM dbo.Contact
                   WHERE Id = @id;";

            return await SqlRunner.QueryFirstAsync<ContactEntity>(connectionString, sql, new { id });
        }
    }
}
