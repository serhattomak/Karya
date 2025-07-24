using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Product.Dto;
using Karya.Domain.Enums;

namespace Karya.Application.Features.Page.Dto;

public class PageDto
{
	public Guid Id { get; set; }
	public PageTypes PageType { get; set; }
	public string Name { get; set; } = string.Empty;
	public List<string> Titles { get; set; } = [];
	public List<string>? Subtitles { get; set; } = [];
	public List<string>? Descriptions { get; set; } = [];
	public List<string>? ListItems { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public string? BackgroundImageUrl { get; set; }
	public string? BannerImageUrl { get; set; }
	public List<FileDto> Files { get; set; } = [];
	public List<ProductDto> Products { get; set; } = [];
	public List<string>? AdditionalFields { get; set; } = [];
}