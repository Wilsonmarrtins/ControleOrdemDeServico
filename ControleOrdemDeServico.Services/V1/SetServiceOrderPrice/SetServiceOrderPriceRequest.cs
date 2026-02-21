namespace OsService.Services.V1.SetServiceOrderPrice;

public sealed record SetServiceOrderPriceRequest(decimal? Price, string? Coin);