using Dapper;
using OsService.Domain.Entities;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class CustomerRepository(IDefaultSqlConnectionFactory factory) : ICustomerRepository
{
    public async Task InsertAsync(CustomerEntity customer, CancellationToken ct)
    {
        const string sql = """
        INSERT INTO dbo.Customers (Id, Name, Phone, Email, Document, CreatedAt)
        VALUES (@Id, @Name, @Phone, @Email, @Document, @CreatedAt);
        """;

        using var conn = factory.Create();
        await conn.ExecuteAsync(new CommandDefinition(sql, customer, cancellationToken: ct));
    }

    public async Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = """
        SELECT TOP 1 Id, Name, Phone, Email, Document, CreatedAt
        FROM dbo.Customers
        WHERE Id = @Id;
        """;

        using var conn = factory.Create();
        return await conn.QuerySingleOrDefaultAsync<CustomerEntity>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }


    public async Task<CustomerEntity?> GetByPhoneAsync(string phone, CancellationToken ct)
    {
        const string sql = """
    SELECT TOP 1
        Id,
        Name,
        Phone,
        Email,
        Document,
        CreatedAt
    FROM dbo.Customers
    WHERE Phone = @Phone;
    """;

        using var conn = factory.Create();

        return await conn.QuerySingleOrDefaultAsync<CustomerEntity>(
            new CommandDefinition(sql, new { Phone = phone }, cancellationToken: ct));
    }

    public async Task<CustomerEntity?> GetByDocumentAsync(string document, CancellationToken ct)
    {
        const string sql = """
    SELECT TOP 1
        Id,
        Name,
        Phone,
        Email,
        Document,
        CreatedAt
    FROM dbo.Customers
    WHERE Document = @Document;
    """;

        using var conn = factory.Create();

        return await conn.QuerySingleOrDefaultAsync<CustomerEntity>(
            new CommandDefinition(sql, new { Document = document }, cancellationToken: ct));
    }
}