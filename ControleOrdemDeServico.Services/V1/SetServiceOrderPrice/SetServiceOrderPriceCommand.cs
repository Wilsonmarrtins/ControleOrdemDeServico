using MediatR;

namespace OsService.Services.V1.SetServiceOrderPrice;

public sealed record SetServiceOrderPriceCommand(Guid Id, decimal? Price, string? Coin) : IRequest<bool>;