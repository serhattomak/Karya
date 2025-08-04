using Karya.Domain.Common;

namespace Karya.Domain.Entities;

public class Product : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public List<string> Titles { get; set; } = [];
	public List<string>? Subtitles { get; set; } = [];
	public List<string>? Descriptions { get; set; } = [];
	public List<string>? ListTitles { get; set; } = [];
	public List<string>? ListItems { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public string? BannerImageUrl { get; set; }
	public Guid? ProductImageId { get; set; }
	public List<Guid>? DocumentImageIds { get; set; } = [];
	public List<Guid>? ProductDetailImageIds { get; set; } = [];
	public List<Guid>? FileIds { get; set; } = [];
	public List<Guid>? DocumentIds { get; set; } = [];
}