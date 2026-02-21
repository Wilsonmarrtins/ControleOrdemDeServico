using MediatR;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetCustomerById;

public sealed class GetCustomerByIdHandler(ICustomerRepository repo)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return null;

        return new CustomerDto(
            entity.Id,
            entity.Name,
            entity.Phone,
            entity.Email,
            entity.Document,
            entity.CreatedAt
        );
    }
}