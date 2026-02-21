namespace OsService.Services.V1.GetCustomerById;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    string? Document,
    DateTime CreatedAt
);