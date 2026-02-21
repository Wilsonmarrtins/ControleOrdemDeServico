using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;

namespace OsService.Tests.TestDoubles;

public sealed class FakeServiceOrderRepository : IServiceOrderRepository
{
    private readonly Dictionary<Guid, ServiceOrderEntity> _byId = new();
    private int _nextNumber = 1;

    public Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_byId.TryGetValue(id, out var so) ? so : null);

    public Task<(Guid id, int number)> InsertAndReturnNumberAsync(ServiceOrderEntity so, CancellationToken ct)
    {
        var entity = Clone(so, number: _nextNumber++);

        _byId[entity.Id] = entity;

        return Task.FromResult((entity.Id, entity.Number));
    }

    public Task<bool> UpdateStatusAsync(
        Guid id,
        ServiceOrderStatus newStatus,
        DateTime? startedAt,
        DateTime? finishedAt,
        CancellationToken ct)
    {
        if (!_byId.TryGetValue(id, out var so))
            return Task.FromResult(false);

        var updated = Clone(
            so,
            status: newStatus,
            startedAt: startedAt ?? so.StartedAt,
            finishedAt: finishedAt ?? so.FinishedAt
        );

        _byId[id] = updated;

        return Task.FromResult(true);
    }

    public Task<bool> UpdatePriceAsync(
        Guid id,
        decimal? price,
        string coin,
        DateTime? updatedAt,
        CancellationToken ct)
    {
        if (!_byId.TryGetValue(id, out var so))
            return Task.FromResult(false);

        var updated = Clone(
            so,
            price: price,
            coin: coin,
            updatedPriceAt: updatedAt
        );

        _byId[id] = updated;

        return Task.FromResult(true);
    }

    public void Seed(ServiceOrderEntity so) => _byId[so.Id] = so;

    private static ServiceOrderEntity Clone(
        ServiceOrderEntity so,
        Guid? id = null,
        int? number = null,
        Guid? customerId = null,
        string? description = null,
        ServiceOrderStatus? status = null,
        DateTime? openedAt = null,
        decimal? price = null,
        string? coin = null,
        DateTime? updatedPriceAt = null,
        DateTime? startedAt = null,
        DateTime? finishedAt = null)
    {
        return new ServiceOrderEntity
        {
            Id = id ?? so.Id,
            Number = number ?? so.Number,
            CustomerId = customerId ?? so.CustomerId,
            Description = description ?? so.Description,
            Status = status ?? so.Status,
            OpenedAt = openedAt ?? so.OpenedAt,
            Price = price ?? so.Price,
            Coin = coin ?? so.Coin,
            UpdatedPriceAt = updatedPriceAt ?? so.UpdatedPriceAt,
            StartedAt = startedAt ?? so.StartedAt,
            FinishedAt = finishedAt ?? so.FinishedAt
        };
    }
}