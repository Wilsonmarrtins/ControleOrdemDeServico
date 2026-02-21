using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.ApiService.Controllers.Requests;
using OsService.Domain.Enums;
using OsService.Services.V1.ChangeServiceOrderStatus;
using OsService.Services.V1.GetServiceOrderById;
using OsService.Services.V1.ListServiceOrderAttachments;
using OsService.Services.V1.OpenServiceOrder;
using OsService.Services.V1.SetServiceOrderPrice;
using OsService.Services.V1.UploadServiceOrderAttachment;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("v1/service-orders")]
public sealed class ServiceOrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Open([FromBody] OpenServiceOrderCommand cmd, CancellationToken ct)
    {
        var (id, number) = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id, number });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await mediator.Send(new GetServiceOrderByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeServiceOrderStatusRequest body,
        CancellationToken ct)
    {
        var ok = await mediator.Send(new ChangeServiceOrderStatusCommand(id, body.Status), ct);
        return ok ? Ok() : NotFound();
    }

    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> SetPrice(
        Guid id,
        [FromBody] SetServiceOrderPriceRequest body,
        CancellationToken ct)
    {
        var ok = await mediator.Send(new SetServiceOrderPriceCommand(id, body.Price, body.Coin), ct);
        return ok ? Ok() : NotFound();
    }

    [HttpPost("{id:guid}/attachments/before")]
    [Consumes("multipart/form-data")]
    public Task<IActionResult> UploadBefore(
        Guid id,
        [FromForm] UploadAttachmentRequest body,
        CancellationToken ct)
        => UploadAttachment(id, body.File, AttachmentType.Before, ct);

    [HttpPost("{id:guid}/attachments/after")]
    [Consumes("multipart/form-data")]
    public Task<IActionResult> UploadAfter(
        Guid id,
        [FromForm] UploadAttachmentRequest body,
        CancellationToken ct)
        => UploadAttachment(id, body.File, AttachmentType.After, ct);

    [HttpGet("{id:guid}/attachments")]
    public async Task<IActionResult> ListAttachments(Guid id, CancellationToken ct)
    {
        var list = await mediator.Send(new ListServiceOrderAttachmentsQuery(id), ct);
        return Ok(list);
    }

    private async Task<IActionResult> UploadAttachment(
        Guid id,
        IFormFile? file,
        AttachmentType type,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "File is required" });

        var dto = await mediator.Send(new UploadServiceOrderAttachmentCommand(id, type, file), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}