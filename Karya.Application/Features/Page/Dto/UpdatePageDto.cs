using Karya.Domain.Enums;

namespace Karya.Application.Features.Page.Dto;

public record UpdatePageDto(
	Guid Id,
	PageTypes PageType,
	string Name,
	List<string> Titles,
	List<string> Descriptions,
	List<string>? Urls,
	List<Guid>? FileIds,
	List<Guid>? ProductIds,
	List<string>? AdditionalFields
	);