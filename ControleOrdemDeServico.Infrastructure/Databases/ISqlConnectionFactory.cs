using System.Data;

namespace OsService.Infrastructure.Databases;

public interface IDefaultSqlConnectionFactory
{
    IDbConnection Create();
}