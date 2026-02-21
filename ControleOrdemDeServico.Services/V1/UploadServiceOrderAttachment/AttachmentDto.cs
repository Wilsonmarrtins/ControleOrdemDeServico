using OsService.Domain.Enums;

namespace OsService.Services.V1.UploadServiceOrderAttachment;

public sealed record AttachmentDto(
    Guid Id,
    Guid ServiceOrderId,
    AttachmentType Type,
    string FileName,
    string ContentType,
    long SizeBytes,
    string StoragePath,
    DateTime UploadedAt
);