using OsService.Domain.Entities;
using OsService.Domain.Enums;

namespace OsService.Infrastructure.Repository;

public interface IServiceOrderAttachmentRepository
{
    Task InsertAsync(ServiceOrderAttachmentEntity entity, CancellationToken ct);
    Task<IReadOnlyList<ServiceOrderAttachmentEntity>> ListByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct);
}