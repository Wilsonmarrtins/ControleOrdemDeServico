using MediatR;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.UploadServiceOrderAttachment;

namespace OsService.Services.V1.ListServiceOrderAttachments;

public sealed class ListServiceOrderAttachmentsHandler(IServiceOrderAttachmentRepository repo)
    : IRequestHandler<ListServiceOrderAttachmentsQuery, IReadOnlyList<AttachmentDto>>
{
    public async Task<IReadOnlyList<AttachmentDto>> Handle(ListServiceOrderAttachmentsQuery request, CancellationToken ct)
    {
        var list = await repo.ListByServiceOrderIdAsync(request.ServiceOrderId, ct);

        return [.. list.Select(x => new AttachmentDto(
            x.Id,
            x.ServiceOrderId,
            x.Type,
            x.FileName,
            x.ContentType,
            x.SizeBytes,
            x.StoragePath,
            x.UploadedAt
        ))];
    }
}