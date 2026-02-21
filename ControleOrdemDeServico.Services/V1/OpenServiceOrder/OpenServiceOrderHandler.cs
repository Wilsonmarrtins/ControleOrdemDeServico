using MediatR;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.OpenServiceOrder;

public sealed class OpenServiceOrderHandler(
    ICustomerRepository customers,
    IServiceOrderRepository serviceOrders)
    : IRequestHandler<OpenServiceOrderCommand, (Guid Id, int Number)>
{
    public async Task<(Guid Id, int Number)> Handle(
        OpenServiceOrderCommand request,
        CancellationToken ct)
    {
        _ = await customers.GetByIdAsync(request.CustomerId, ct)
            ?? throw new KeyNotFoundException("Customer not found");

        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required");

        if (description.Length > 500)
            throw new ArgumentException("Description must be up to 500 characters");

        var entity = new ServiceOrderEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Description = description,
            Status = ServiceOrderStatus.Open,
            OpenedAt = DateTime.UtcNow
        };

        return await serviceOrders.InsertAndReturnNumberAsync(entity, ct);
    }
}