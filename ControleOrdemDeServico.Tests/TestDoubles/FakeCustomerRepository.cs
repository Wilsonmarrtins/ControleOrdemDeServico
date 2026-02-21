using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Tests.TestDoubles;

public sealed class FakeCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<Guid, CustomerEntity> _byId = new();

    public Task InsertAsync(CustomerEntity customer, CancellationToken ct)
    {
        _byId[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_byId.TryGetValue(id, out var c) ? c : null);

    public Task<CustomerEntity?> GetByPhoneAsync(string phone, CancellationToken ct)
        => Task.FromResult(_byId.Values.FirstOrDefault(x => x.Phone == phone));

    public Task<CustomerEntity?> GetByDocumentAsync(string document, CancellationToken ct)
        => Task.FromResult(_byId.Values.FirstOrDefault(x => x.Document == document));
    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_byId.ContainsKey(id));
}