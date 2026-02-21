using Dapper;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class ServiceOrderAttachmentRepository(IDefaultSqlConnectionFactory factory) : IServiceOrderAttachmentRepository
{
    public async Task InsertAsync(ServiceOrderAttachmentEntity entity, CancellationToken ct)
    {
        const string sql = """
        INSERT INTO dbo.ServiceOrderAttachments
            (Id, ServiceOrderId, Type, FileName, ContentType, SizeBytes, StoragePath, UploadedAt)
        VALUES
            (@Id, @ServiceOrderId, @Type, @FileName, @ContentType, @SizeBytes, @StoragePath, @UploadedAt);
        """;

        using var conn = factory.Create();
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            entity.Id,
            entity.ServiceOrderId,
            Type = (int)entity.Type,
            entity.FileName,
            entity.ContentType,
            entity.SizeBytes,
            entity.StoragePath,
            entity.UploadedAt
        }, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<ServiceOrderAttachmentEntity>> ListByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct)
    {
        const string sql = """
        SELECT
            Id,
            ServiceOrderId,
            Type,
            FileName,
            ContentType,
            SizeBytes,
            StoragePath,
            UploadedAt
        FROM dbo.ServiceOrderAttachments
        WHERE ServiceOrderId = @ServiceOrderId
        ORDER BY UploadedAt DESC;
        """;

        using var conn = factory.Create();

        var rows = await conn.QueryAsync<dynamic>(new CommandDefinition(sql, new { ServiceOrderId = serviceOrderId }, cancellationToken: ct));

        return rows.Select(r => new ServiceOrderAttachmentEntity
        {
            Id = r.Id,
            ServiceOrderId = r.ServiceOrderId,
            Type = (AttachmentType)(int)r.Type,
            FileName = r.FileName,
            ContentType = r.ContentType,
            SizeBytes = r.SizeBytes,
            StoragePath = r.StoragePath,
            UploadedAt = r.UploadedAt
        }).ToList();
    }
}