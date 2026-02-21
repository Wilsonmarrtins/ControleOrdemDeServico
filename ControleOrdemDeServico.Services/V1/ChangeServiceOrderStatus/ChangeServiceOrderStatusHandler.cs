using MediatR;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.ChangeServiceOrderStatus;

public sealed class ChangeServiceOrderStatusHandler(IServiceOrderRepository repo)
    : IRequestHandler<ChangeServiceOrderStatusCommand, bool>
{
    public async Task<bool> Handle(ChangeServiceOrderStatusCommand request, CancellationToken ct)
    {
        var so = await repo.GetByIdAsync(request.Id, ct);

        if (so is null)
            return false;

        var current = so.Status;
        var next = request.Status;

        if (current == ServiceOrderStatus.Open &&
            next == ServiceOrderStatus.InProgress)
        {
            return await repo.UpdateStatusAsync(
                request.Id,
                next,
                startedAt: DateTime.UtcNow,
                finishedAt: null,
                ct);
        }

        if (current == ServiceOrderStatus.InProgress &&
            next == ServiceOrderStatus.Finished)
        {
            return await repo.UpdateStatusAsync(
                request.Id,
                next,
                startedAt: null,
                finishedAt: DateTime.UtcNow,
                ct);
        }


        throw new InvalidOperationException("Invalid status transition");
    }
}