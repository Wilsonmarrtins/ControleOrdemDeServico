using MediatR;
using Microsoft.AspNetCore.Http;
using OsService.Domain.Enums;

namespace OsService.Services.V1.UploadServiceOrderAttachment;

public sealed record UploadServiceOrderAttachmentCommand(
    Guid ServiceOrderId,
    AttachmentType Type,
    IFormFile File
) : IRequest<AttachmentDto?>;