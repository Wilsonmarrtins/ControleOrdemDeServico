using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Services.V1.CreateCustomer;
using OsService.Services.V1.GetCustomerById;
using OsService.Services.V1.SearchCustomer;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("v1/customers")]
public sealed class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await mediator.Send(new GetCustomerByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? phone,
        [FromQuery] string? document,
        CancellationToken ct)
    {
        var dto = await mediator.Send(new SearchCustomerQuery(phone, document), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}