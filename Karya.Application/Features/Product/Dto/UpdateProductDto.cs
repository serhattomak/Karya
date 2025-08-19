﻿namespace Karya.Application.Features.Product.Dto;

public record UpdateProductDto(
	Guid Id,
	string Name,
	string Slug,
	string? HomePageSubtitle,
	List<string> Titles,
	List<string>? Subtitles,
	List<string>? Descriptions,
	List<string>? ListTitles,
	List<string>? ListItems,
	List<string>? Urls,
	List<string>? VideoUrls,
	List<string>? VideoDescriptions,
	string? BannerImageUrl,
	string? MainImageUrl,
	bool ShowContact,
	Guid? ProductImageId,
	Guid? ProductMainImageId,
	List<Guid>? DocumentImageIds,
	List<Guid>? ProductDetailImageIds,
	List<Guid>? FileIds,
	List<Guid>? DocumentIds);