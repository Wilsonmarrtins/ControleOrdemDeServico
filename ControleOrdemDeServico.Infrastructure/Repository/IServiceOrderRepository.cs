using OsService.Domain.Entities;
using OsService.Domain.Enums;

namespace OsService.Infrastructure.Repository;

public interface IServiceOrderRepository
{
    Task<(Guid id, int number)> InsertAndReturnNumberAsync(ServiceOrderEntity so, CancellationToken ct);
    Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> UpdateStatusAsync(Guid id, ServiceOrderStatus newStatus, DateTime? startedAt, DateTime? finishedAt, CancellationToken ct);
    Task<bool> UpdatePriceAsync(Guid id, decimal? price, string coin, DateTime? updatedAt, CancellationToken ct);
}