using Karya.Domain.Common;

namespace Karya.Domain.Entities;

public class Document : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Url { get; set; }
	public Guid? FileId { get; set; }
	public string? PreviewImageUrl { get; set; }
	public Guid? PreviewImageFileId { get; set; }
	public string? Category { get; set; }
	public int? Order { get; set; }
	public bool IsActive { get; set; } = true;
	public string? MimeType { get; set; }
	public long? FileSize { get; set; }
	public int? DownloadCount { get; set; } = 0;
	public DateTime? LastDownloadDate { get; set; }
}