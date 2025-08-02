namespace Karya.Application.Features.Product.Dto;

public record UpdateProductDto(
	Guid Id,
	string Name,
	List<string> Titles,
	List<string>? Subtitles,
	List<string>? Descriptions,
	List<string>? ListTitles,
	List<string>? ListItems,
	List<string>? Urls,
	string? BannerImageUrl,
	Guid? ProductImageId,
	List<Guid>? DocumentImageIds,
	List<Guid>? ProductDetailImageIds,
	List<Guid>? FileIds);