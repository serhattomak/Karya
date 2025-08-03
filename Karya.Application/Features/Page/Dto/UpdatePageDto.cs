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
	string? BackgroundImageUrl,
	string? BannerImageUrl,
	List<Guid>? FileIds,
	List<Guid>? ProductIds,
	List<string>? AdditionalFields
);