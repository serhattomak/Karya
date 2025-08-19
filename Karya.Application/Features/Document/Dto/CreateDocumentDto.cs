namespace Karya.Application.Features.Document.Dto;

public record CreateDocumentDto(
	string Name,
	string Slug,
	string? Description,
	string? Url,
	Guid? FileId,
	string? PreviewImageUrl,
	Guid? PreviewImageFileId,
	string? Category,
	int? Order,
	bool IsActive = true,
	string? MimeType = null,
	long? FileSize = null
);