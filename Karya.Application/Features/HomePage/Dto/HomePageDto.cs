using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Product.Dto;

namespace Karya.Application.Features.HomePage.Dto;

public class HomePageDto
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string BackgroundImageUrl { get; set; } = string.Empty;
	public List<FileDto> Files { get; set; } = [];
	public List<ProductDto> Products { get; set; } = [];
}