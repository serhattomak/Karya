using Karya.Domain.Common;
using Karya.Domain.Enums;

namespace Karya.Domain.Entities;

public class Page : BaseEntity
{
	public PageTypes PageType { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public List<string> Titles { get; set; } = [];
	public List<string>? Subtitles { get; set; }
	public List<string>? Descriptions { get; set; }
	public List<string>? ListTitles { get; set; } = [];
	public List<string>? ListItems { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public List<string>? VideoTitles { get; set; }
	public List<string>? VideoUrls { get; set; } = [];
	public List<string>? VideoDescriptions { get; set; }
	public string? BackgroundImageUrl { get; set; }
	public string? BannerImageUrl { get; set; }
	public string? MainImageUrl { get; set; }
	public Guid? MainImageId { get; set; }
	public List<Guid>? FileIds { get; set; } = [];
	public List<Guid>? ProductIds { get; set; } = [];
	public List<string>? AdditionalFields { get; set; } = [];
	public List<Guid>? DocumentIds { get; set; } = [];
}