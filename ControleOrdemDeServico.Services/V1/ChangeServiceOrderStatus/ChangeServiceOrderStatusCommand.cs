using MediatR;
using OsService.Domain.Enums;

namespace OsService.Services.V1.ChangeServiceOrderStatus;

public sealed record ChangeServiceOrderStatusCommand(
    Guid Id,
    ServiceOrderStatus Status
) : IRequest<bool>;