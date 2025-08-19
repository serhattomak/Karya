using Karya.Application.Features.Document.Dto;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Product.Dto;
using Karya.Domain.Enums;

namespace Karya.Application.Features.Page.Dto;

public class PageDto
{
	public Guid Id { get; set; }
	public PageTypes PageType { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public List<string> Titles { get; set; } = [];
	public List<string>? Subtitles { get; set; } = [];
	public List<string>? Descriptions { get; set; } = [];
	public List<string>? ListTitles { get; set; } = [];
	public List<string>? ListItems { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public List<string>? VideoTitles { get; set; } = [];
	public List<string>? VideoUrls { get; set; } = [];
	public List<string>? VideoDescriptions { get; set; } = [];
	public string? BackgroundImageUrl { get; set; }
	public string? BannerImageUrl { get; set; }
	public string? MainImageUrl { get; set; }
	public Guid? MainImageId { get; set; }
	public FileDto? MainImage { get; set; }
	public List<Guid>? FileIds { get; set; } = [];
	public List<Guid>? ProductIds { get; set; } = [];
	public List<Guid>? DocumentIds { get; set; } = [];
	public List<string>? AdditionalFields { get; set; } = [];
	public List<FileDto> Files { get; set; } = [];
	public List<ProductDto> Products { get; set; } = [];
	public List<DocumentDto> Documents { get; set; } = [];
}