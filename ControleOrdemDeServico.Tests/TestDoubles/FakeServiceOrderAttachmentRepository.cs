using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Tests.TestDoubles;

public sealed class FakeServiceOrderAttachmentRepository : IServiceOrderAttachmentRepository
{
    private readonly List<ServiceOrderAttachmentEntity> _items = [];

    public Task InsertAsync(ServiceOrderAttachmentEntity entity, CancellationToken ct)
    {
        _items.Add(entity);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ServiceOrderAttachmentEntity>> ListByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct)
        => Task.FromResult((IReadOnlyList<ServiceOrderAttachmentEntity>)_items.Where(x => x.ServiceOrderId == serviceOrderId).ToList());
}