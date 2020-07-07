﻿using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal static class IntegrationDatabase
    {
        public static async Task ResetAsync(IConfiguration config)
        {
            using IDbConnection databaseConnection = new SqlConnection(config.GetConnectionString("OrderingDbAdminConnectionString"));
            await databaseConnection.ExecuteAsync("GRANT CONNECT TO [NHSD-ORDAPI];");
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader ADD MEMBER [NHSD-ORDAPI];");
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter ADD MEMBER [NHSD-ORDAPI];");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.OrderItem;");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.ServiceRecipient;");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.[Order];");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.[Address];");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.Contact;");
        }

        public static async Task RemoveReadRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader DROP MEMBER [NHSD-ORDAPI];");
        }

        public static async Task RemoveWriteRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter DROP MEMBER [NHSD-ORDAPI];");
        }

        public static async Task DenyAccessForNhsdUser(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("DENY CONNECT TO [NHSD-ORDAPI];");
        }
    }
}
