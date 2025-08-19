using Karya.Application.Features.Document.Dto;
using Karya.Application.Features.File.Dto;

namespace Karya.Application.Features.Product.Dto;

public class ProductDto()
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? HomePageSubtitle { get; set; }
	public List<string> Titles { get; set; } = [];
	public List<string>? Subtitles { get; set; } = [];
	public List<string> Descriptions { get; set; } = [];
	public List<string>? ListTitles { get; set; } = [];
	public List<string>? ListItems { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public List<string>? VideoTitles { get; set; } = [];
	public List<string>? VideoUrls { get; set; } = [];
	public List<string>? VideoDescriptions { get; set; } = [];
	public string? BannerImageUrl { get; set; }
	public string? MainImageUrl { get; set; }
	public bool ShowContact { get; set; } = false;
	public Guid? ProductImageId { get; set; }
	public Guid? ProductMainImageId { get; set; }
	public List<Guid>? DocumentImageIds { get; set; } = [];
	public List<Guid>? ProductDetailImageIds { get; set; } = [];
	public List<Guid> FileIds { get; set; } = [];
	public List<Guid>? DocumentIds { get; set; } = [];
	public List<FileDto> Files { get; set; } = [];
	public FileDto? ProductImage { get; set; }
	public FileDto? ProductMainImage { get; set; }
	public List<FileDto> DocumentImages { get; set; } = [];
	public List<FileDto> ProductImages { get; set; } = [];
	public List<DocumentDto> Documents { get; set; } = [];
}