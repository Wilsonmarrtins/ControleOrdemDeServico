using MediatR;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetServiceOrderById;

public sealed class GetServiceOrderByIdHandler(IServiceOrderRepository repo)
    : IRequestHandler<GetServiceOrderByIdQuery, ServiceOrderDto?>
{
    public async Task<ServiceOrderDto?> Handle(GetServiceOrderByIdQuery request, CancellationToken ct)
    {
        var so = await repo.GetByIdAsync(request.Id, ct);
        if (so is null) return null;

        return new ServiceOrderDto(
            so.Id,
            so.Number,
            so.CustomerId,
            so.Description,
            so.Status,
            so.OpenedAt,
            so.Price,
            so.Coin,
            so.UpdatedPriceAt,
            so.StartedAt,
            so.FinishedAt
        );
    }
}