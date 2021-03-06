﻿using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal static class IntegrationDatabase
    {
        public static async Task ResetAsync(IConfiguration config)
        {
            await using var databaseConnection = new SqlConnection(
                config.GetConnectionString("OrderingDbAdminConnectionString"));

            await databaseConnection.ExecuteAsync("GRANT CONNECT TO [NHSD-ORDAPI];");

            // ReSharper disable StringLiteralTypo
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader ADD MEMBER [NHSD-ORDAPI];");
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter ADD MEMBER [NHSD-ORDAPI];");

            // ReSharper restore StringLiteralTypo
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.[Order];");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.OrderingParty;");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.Supplier;");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.[Address];");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.CatalogueItem;");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.Contact;");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.PricingUnit;");
            await databaseConnection.ExecuteAsync("DELETE FROM dbo.ServiceRecipient;");
            await databaseConnection.ExecuteAsync("DBCC CHECKIDENT ('dbo.[Order]', RESEED, 10000);");

            // ReSharper restore StringLiteralTypo
        }

        public static async Task RemoveReadRoleAsync(string connectionString)
        {
            await using var databaseConnection = new SqlConnection(connectionString);

            // ReSharper disable once StringLiteralTypo
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader DROP MEMBER [NHSD-ORDAPI];");
        }

        public static async Task RemoveWriteRoleAsync(string connectionString)
        {
            await using var databaseConnection = new SqlConnection(connectionString);

            // ReSharper disable once StringLiteralTypo
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter DROP MEMBER [NHSD-ORDAPI];");
        }

        public static async Task DenyAccessForNhsdUser(string connectionString)
        {
            await using var databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("DENY CONNECT TO [NHSD-ORDAPI];");
        }
    }
}
