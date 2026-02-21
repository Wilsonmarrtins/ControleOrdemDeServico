using MediatR;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.GetCustomerById;

namespace OsService.Services.V1.SearchCustomer;

public sealed class SearchCustomerHandler(ICustomerRepository repo)
    : IRequestHandler<SearchCustomerQuery, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(SearchCustomerQuery request, CancellationToken ct)
    {
        var phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        var document = string.IsNullOrWhiteSpace(request.Document) ? null : request.Document.Trim();

        if (phone is null && document is null)
            return null;

        var entity =
            document is not null ? await repo.GetByDocumentAsync(document, ct)
          : await repo.GetByPhoneAsync(phone!, ct);

        return entity is null
            ? null
            : new CustomerDto(entity.Id, entity.Name, entity.Phone, entity.Email, entity.Document, entity.CreatedAt);
    }
}