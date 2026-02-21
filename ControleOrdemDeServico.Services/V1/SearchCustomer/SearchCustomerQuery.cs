using MediatR;
using OsService.Services.V1.GetCustomerById;

namespace OsService.Services.V1.SearchCustomer;

public sealed record SearchCustomerQuery(string? Phone, string? Document) : IRequest<CustomerDto?>;