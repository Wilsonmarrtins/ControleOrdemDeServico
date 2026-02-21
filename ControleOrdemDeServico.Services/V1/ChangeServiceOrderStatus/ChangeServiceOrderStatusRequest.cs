using OsService.Domain.Enums;

namespace OsService.Services.V1.ChangeServiceOrderStatus;

public sealed record ChangeServiceOrderStatusRequest(ServiceOrderStatus Status);