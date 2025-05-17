namespace Karya.Application.Features.AboutPage.Dto;

public record CreateAboutPageDto(
	string Name,
	List<string> Titles,
	List<string> Descriptions,
	List<string>? Urls,
	List<Guid> FileIds
);