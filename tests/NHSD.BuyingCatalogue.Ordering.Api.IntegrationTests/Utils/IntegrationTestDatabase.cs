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
            using IDbConnection databaseConnection = new SqlConnection(config.GetConnectionString("OrderingDbAdmin"));
            await databaseConnection.ExecuteAsync("GRANT CONNECT TO NHSD;");
            await databaseConnection.ExecuteAsync("DELETE FROM [dbo].[Order];");
        }

        public static async Task RemoveReadRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader DROP MEMBER NHSD;");
        }

        public static async Task RemoveWriteRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter DROP MEMBER NHSD;");
        }
    }
}

