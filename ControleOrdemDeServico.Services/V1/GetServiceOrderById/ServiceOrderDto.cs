using OsService.Domain.Enums;

namespace OsService.Services.V1.GetServiceOrderById;

public sealed record ServiceOrderDto(
    Guid Id,
    int Number,
    Guid CustomerId,
    string Description,
    ServiceOrderStatus Status,
    DateTime OpenedAt,
    decimal? Price,
    string Coin,
    DateTime? UpdatedPriceAt,
    DateTime? StartedAt,
    DateTime? FinishedAt
);