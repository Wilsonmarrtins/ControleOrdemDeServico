using MediatR;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.SetServiceOrderPrice;

public sealed class SetServiceOrderPriceHandler(IServiceOrderRepository repo)
    : IRequestHandler<SetServiceOrderPriceCommand, bool>
{
    public async Task<bool> Handle(SetServiceOrderPriceCommand request, CancellationToken ct)
    {
        var so = await repo.GetByIdAsync(request.Id, ct);
        if (so is null) return false;

        if (so.Status == ServiceOrderStatus.Finished)
            return false;

        if (request.Price is < 0)
            throw new ArgumentOutOfRangeException(nameof(request.Price), "Price cannot be negative");

        var coinRaw = string.IsNullOrWhiteSpace(request.Coin)
            ? "BRL"
            : request.Coin.Trim().ToUpperInvariant();

        var coin = (coinRaw.Length == 3) ? coinRaw : "BRL";

        return await repo.UpdatePriceAsync(request.Id, request.Price, coin, DateTime.UtcNow, ct);
    }
}