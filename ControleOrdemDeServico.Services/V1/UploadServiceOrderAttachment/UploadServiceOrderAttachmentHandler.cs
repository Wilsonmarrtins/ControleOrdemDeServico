using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.UploadServiceOrderAttachment;

public sealed class UploadServiceOrderAttachmentHandler(
    IServiceOrderRepository serviceOrderRepo,
    IServiceOrderAttachmentRepository attachmentRepo)
    : IRequestHandler<UploadServiceOrderAttachmentCommand, AttachmentDto?>
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png" };

    public async Task<AttachmentDto?> Handle(UploadServiceOrderAttachmentCommand request, CancellationToken ct)
    {
        var so = await serviceOrderRepo.GetByIdAsync(request.ServiceOrderId, ct);
        if (so is null)
            return null;

        if (request.File is null || request.File.Length == 0)
            return null;

        if (request.File.Length > MaxBytes)
            throw new ArgumentException("File too large");

        var originalFileName = Path.GetFileName(request.File.FileName);
        var ext = Path.GetExtension(originalFileName);

        if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            throw new ArgumentException("Invalid file extension. Only JPG/PNG are allowed.");

        var contentType = request.File.ContentType ?? "application/octet-stream";
        if (!AllowedContentTypes.Contains(contentType))
            throw new ArgumentException("Invalid content-type. Only image/jpeg or image/png are allowed.");

        var uploadsRoot = Environment.GetEnvironmentVariable("OS_UPLOAD_ROOT");
        if (string.IsNullOrWhiteSpace(uploadsRoot))
            uploadsRoot = "/data/uploads";

        Directory.CreateDirectory(uploadsRoot);

        var storedName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsRoot, storedName);

        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await request.File.CopyToAsync(fs, ct);
        }

        var entity = new ServiceOrderAttachmentEntity
        {
            Id = Guid.NewGuid(),
            ServiceOrderId = request.ServiceOrderId,
            Type = request.Type,
            FileName = originalFileName,
            ContentType = contentType,
            SizeBytes = request.File.Length,
            StoragePath = fullPath,
            UploadedAt = DateTime.UtcNow
        };

        await attachmentRepo.InsertAsync(entity, ct);

        return new AttachmentDto(
            entity.Id,
            entity.ServiceOrderId,
            entity.Type,
            entity.FileName,
            entity.ContentType,
            entity.SizeBytes,
            entity.StoragePath,
            entity.UploadedAt
        );
    }
}