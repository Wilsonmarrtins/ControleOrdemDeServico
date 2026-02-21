using Microsoft.AspNetCore.Http;

namespace OsService.ApiService.Controllers.Requests;

public sealed class UploadAttachmentRequest
{
    public IFormFile File { get; init; } = default!;
}