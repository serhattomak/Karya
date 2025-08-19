using Karya.Domain.Enums;

namespace Karya.Application.Features.Page.Dto;

public record UpdatePageDto(
	Guid Id,
	PageTypes PageType,
	string Name,
	string Slug,
	List<string> Titles,
	List<string>? Subtitles,
	List<string>? Descriptions,
	List<string>? ListTitles,
	List<string>? ListItems,
	List<string>? Urls,
	List<string>? VideoTitles,
	List<string>? VideoUrls,
	List<string>? VideoDescriptions,
	string? BackgroundImageUrl,
	string? BannerImageUrl,
	string? MainImageUrl,
	Guid? MainImageId,
	List<Guid>? FileIds,
	List<Guid>? ProductIds,
	List<Guid>? DocumentIds,
	List<string>? AdditionalFields
);