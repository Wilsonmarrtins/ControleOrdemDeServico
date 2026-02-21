using MediatR;
using OsService.Services.V1.UploadServiceOrderAttachment;

namespace OsService.Services.V1.ListServiceOrderAttachments;

public sealed record ListServiceOrderAttachmentsQuery(Guid ServiceOrderId) : IRequest<IReadOnlyList<AttachmentDto>>;