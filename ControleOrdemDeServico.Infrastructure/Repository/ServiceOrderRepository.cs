using Dapper;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class ServiceOrderRepository(IDefaultSqlConnectionFactory factory) : IServiceOrderRepository
{
    public async Task<(Guid id, int number)> InsertAndReturnNumberAsync(ServiceOrderEntity so, CancellationToken ct)
    {
        const string sql = """
        INSERT INTO dbo.ServiceOrders
            (Id, CustomerId, Description, Status, OpenedAt, Price, Coin, UpdatedPriceAt, StartedAt, FinishedAt)
        OUTPUT INSERTED.Id, INSERTED.Number
        VALUES
            (@Id, @CustomerId, @Description, @Status, @OpenedAt, @Price, @Coin, @UpdatedPriceAt, @StartedAt, @FinishedAt);
        """;

        using var conn = factory.Create();
        return await conn.QuerySingleAsync<(Guid id, int number)>(
            new CommandDefinition(sql, new
            {
                so.Id,
                so.CustomerId,
                so.Description,
                Status = (int)so.Status,
                so.OpenedAt,
                so.Price,
                so.Coin,
                UpdatedPriceAt = so.UpdatedPriceAt,
                so.StartedAt,
                so.FinishedAt
            }, cancellationToken: ct));
    }

    public async Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = """
        SELECT TOP 1
            Id,
            Number,
            CustomerId,
            Description,
            Status,
            OpenedAt,
            Price,
            Coin,
            UpdatedPriceAt as UpdatedPriceAt,
            StartedAt,
            FinishedAt
        FROM dbo.ServiceOrders
        WHERE Id = @Id;
        """;

        using var conn = factory.Create();

        var row = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        if (row is null) return null;

        return new ServiceOrderEntity
        {
            Id = row.Id,
            Number = row.Number,
            CustomerId = row.CustomerId,
            Description = row.Description,
            Status = (ServiceOrderStatus)(int)row.Status,
            OpenedAt = row.OpenedAt,
            Price = row.Price,
            Coin = row.Coin,
            UpdatedPriceAt = row.UpdatedPriceAt,
            StartedAt = row.StartedAt,
            FinishedAt = row.FinishedAt
        };
    }

    public async Task<bool> UpdateStatusAsync(Guid id, ServiceOrderStatus newStatus, DateTime? startedAt, DateTime? finishedAt, CancellationToken ct)
    {
        const string sql = """
        UPDATE dbo.ServiceOrders
        SET Status = @Status,
            StartedAt = COALESCE(@StartedAt, StartedAt),
            FinishedAt = COALESCE(@FinishedAt, FinishedAt)
        WHERE Id = @Id;
        """;

        using var conn = factory.Create();
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = id,
            Status = (int)newStatus,
            StartedAt = startedAt,
            FinishedAt = finishedAt
        }, cancellationToken: ct));

        return rows > 0;
    }

    public async Task<bool> UpdatePriceAsync(Guid id, decimal? price, string coin, DateTime? updatedAt, CancellationToken ct)
    {
        const string sql = """
        UPDATE dbo.ServiceOrders
        SET Price = @Price,
            Coin = @Coin,
            UpdatedPriceAt = @UpdatedPriceAt
        WHERE Id = @Id;
        """;

        using var conn = factory.Create();
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = id,
            Price = price,
            Coin = coin,
            UpdatedPriceAt = updatedAt
        }, cancellationToken: ct));

        return rows > 0;
    }
}