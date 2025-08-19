using Karya.Domain.Common;

namespace Karya.Domain.Entities;

public class Product : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? HomePageSubtitle { get; set; }
	public List<string> Titles { get; set; } = [];
	public List<string>? Subtitles { get; set; } = [];
	public List<string>? Descriptions { get; set; } = [];
	public List<string>? ListTitles { get; set; } = [];
	public List<string>? ListItems { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public List<string>? VideoTitles { get; set; } = [];
	public List<string>? VideoUrls { get; set; }
	public List<string>? VideoDescriptions { get; set; }
	public string? BannerImageUrl { get; set; }
	public string? MainImageUrl { get; set; }
	public bool ShowContact { get; set; } = false;
	public Guid? ProductImageId { get; set; }
	public Guid? ProductMainImageId { get; set; }
	public List<Guid>? DocumentImageIds { get; set; } = [];
	public List<Guid>? ProductDetailImageIds { get; set; } = [];
	public List<Guid>? FileIds { get; set; } = [];
	public List<Guid>? DocumentIds { get; set; } = [];
}