using OsService.Domain.Entities;

namespace OsService.Infrastructure.Repository;

public interface ICustomerRepository
{
    Task InsertAsync(CustomerEntity customer, CancellationToken ct);
    Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<CustomerEntity?> GetByPhoneAsync(string phone, CancellationToken ct);
    Task<CustomerEntity?> GetByDocumentAsync(string document, CancellationToken ct);
}