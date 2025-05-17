using Karya.Domain.Common;

namespace Karya.Domain.Entities;

public class HomePage : BaseEntity
{
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string BackgroundImageUrl { get; set; } = string.Empty;
	public List<Guid>? FileIds { get; set; } = [];
	public List<Guid>? ProductIds { get; set; } = [];
}