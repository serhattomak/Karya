using Karya.Application.Features.File.Dto;

namespace Karya.Application.Features.Document.Dto;

public class DocumentDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Url { get; set; }
	public Guid? FileId { get; set; }
	public string? PreviewImageUrl { get; set; }
	public Guid? PreviewImageFileId { get; set; }
	public string? Category { get; set; }
	public int? Order { get; set; }
	public bool IsActive { get; set; }
	public string? MimeType { get; set; }
	public long? FileSize { get; set; }
	public int? DownloadCount { get; set; }
	public DateTime? LastDownloadDate { get; set; }
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }

	public FileDto? File { get; set; }
	public FileDto? PreviewImageFile { get; set; }
}