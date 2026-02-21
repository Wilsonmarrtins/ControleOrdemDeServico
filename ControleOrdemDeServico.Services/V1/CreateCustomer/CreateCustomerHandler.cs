using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;
using System.Text.RegularExpressions;

namespace OsService.Services.V1.CreateCustomer;

public sealed class CreateCustomerHandler(ICustomerRepository repo)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        var email = request.Email?.Trim();

        if (!string.IsNullOrWhiteSpace(email))
        {
            var isValid = Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);

            if (!isValid)
                throw new ArgumentException("Invalid email");
        }

        var phone = request.Phone?.Trim();
        var document = request.Document?.Trim();

        var entity = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Phone = phone,
            Document = document,
            CreatedAt = DateTime.UtcNow
        };

        await repo.InsertAsync(entity, ct);

        return entity.Id;
    }
}