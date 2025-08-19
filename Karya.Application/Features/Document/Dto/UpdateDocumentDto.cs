namespace Karya.Application.Features.Document.Dto;

public record UpdateDocumentDto(
	Guid Id,
	string Name,
	string Slug,
	string? Description,
	string? Url,
	Guid? FileId,
	string? PreviewImageUrl,
	Guid? PreviewImageFileId,
	string? Category,
	int? Order,
	bool IsActive,
	string? MimeType = null,
	long? FileSize = null
);