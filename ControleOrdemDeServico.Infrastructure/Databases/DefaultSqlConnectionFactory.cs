using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace OsService.Infrastructure.Databases;

public sealed class DefaultSqlConnectionFactory(IConfiguration config) : IDefaultSqlConnectionFactory
{
    public IDbConnection Create()
        => new SqlConnection(config.GetConnectionString("Default"));
}